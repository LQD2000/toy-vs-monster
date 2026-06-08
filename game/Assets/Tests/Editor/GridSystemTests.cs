using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class GridSystemTests
{
    private GameObject _gameObject;
    private GridSystem _gridSystem;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject();
        _gridSystem = _gameObject.AddComponent<GridSystem>();
        LogAssert.Expect(LogType.Log, "[GridSystem] 网格初始化完成: 5 行 x 10 列, 原点: (-6.00, -4.00, 0.00)");
        SingletonTestHelper.InvokeAwake(_gridSystem);
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_gameObject);
        SingletonTestHelper.ResetSingleton<GridSystem>();
    }

    [Test]
    public void InitialState_ShouldHaveCorrectDimensions()
    {
        Assert.AreEqual(5, _gridSystem.Rows);
        Assert.AreEqual(10, _gridSystem.Cols);
    }

    [Test]
    public void GridToWorld_Row0Col0_ShouldReturnCenterOfFirstCell()
    {
        Vector3 origin = _gridSystem.GridOrigin;
        float expectedX = origin.x + _gridSystem.CellWidth * 0.5f;
        float expectedY = origin.y - _gridSystem.CellHeight * 0.5f;

        Vector3 result = _gridSystem.GridToWorld(0, 0);

        Assert.AreEqual(expectedX, result.x, 0.001f);
        Assert.AreEqual(expectedY, result.y, 0.001f);
    }

    [Test]
    public void GridToWorld_Row1Col2_ShouldOffsetCorrectly()
    {
        Vector3 origin = _gridSystem.GridOrigin;
        float expectedX = origin.x + 2 * _gridSystem.CellWidth + _gridSystem.CellWidth * 0.5f;
        float expectedY = origin.y - 1 * _gridSystem.CellHeight - _gridSystem.CellHeight * 0.5f;

        Vector3 result = _gridSystem.GridToWorld(1, 2);

        Assert.AreEqual(expectedX, result.x, 0.001f);
        Assert.AreEqual(expectedY, result.y, 0.001f);
    }

    [Test]
    public void WorldToGrid_ValidPosition_ShouldReturnCorrectCell()
    {
        Vector3 worldPos = _gridSystem.GridToWorld(2, 3);

        var result = _gridSystem.WorldToGrid(worldPos);

        Assert.IsTrue(result.HasValue);
        Assert.AreEqual(2, result.Value.row);
        Assert.AreEqual(3, result.Value.col);
    }

    [Test]
    public void WorldToGrid_OutOfBounds_ShouldReturnNull()
    {
        Vector3 outOfBounds = new Vector3(-100f, -100f, 0f);

        var result = _gridSystem.WorldToGrid(outOfBounds);

        Assert.IsFalse(result.HasValue);
    }

    [Test]
    public void OccupyCell_EmptyCell_ShouldReturnTrue()
    {
        GameObject occupant = new GameObject("TestOccupant");

        LogAssert.Expect(LogType.Log, "[GridSystem] 格子 (0, 0) 被占用: TestOccupant");
        bool result = _gridSystem.OccupyCell(0, 0, occupant);

        Assert.IsTrue(result);
        Assert.IsTrue(_gridSystem.IsCellOccupied(0, 0));

        UnityEngine.Object.DestroyImmediate(occupant);
    }

    [Test]
    public void OccupyCell_OccupiedCell_ShouldReturnFalse()
    {
        GameObject occupant1 = new GameObject("Occupant1");
        GameObject occupant2 = new GameObject("Occupant2");

        LogAssert.Expect(LogType.Log, "[GridSystem] 格子 (0, 0) 被占用: Occupant1");
        _gridSystem.OccupyCell(0, 0, occupant1);
        LogAssert.Expect(LogType.Log, "[GridSystem] 格子 (0, 0) 已被占用");
        bool result = _gridSystem.OccupyCell(0, 0, occupant2);

        Assert.IsFalse(result);

        UnityEngine.Object.DestroyImmediate(occupant1);
        UnityEngine.Object.DestroyImmediate(occupant2);
    }

    [Test]
    public void OccupyCell_OutOfBounds_ShouldReturnFalse()
    {
        GameObject occupant = new GameObject("TestOccupant");

        LogAssert.Expect(LogType.Warning, "[GridSystem] 占用格子超出范围: (-1, 0)");
        bool result = _gridSystem.OccupyCell(-1, 0, occupant);

        Assert.IsFalse(result);

        UnityEngine.Object.DestroyImmediate(occupant);
    }

    [Test]
    public void ReleaseCell_OccupiedCell_ShouldClearOccupation()
    {
        GameObject occupant = new GameObject("TestOccupant");
        LogAssert.Expect(LogType.Log, "[GridSystem] 格子 (1, 1) 被占用: TestOccupant");
        _gridSystem.OccupyCell(1, 1, occupant);

        LogAssert.Expect(LogType.Log, "[GridSystem] 释放格子 (1, 1)");
        _gridSystem.ReleaseCell(1, 1);

        Assert.IsFalse(_gridSystem.IsCellOccupied(1, 1));

        UnityEngine.Object.DestroyImmediate(occupant);
    }

    [Test]
    public void IsCellOccupied_OutOfBounds_ShouldReturnTrue()
    {
        LogAssert.Expect(LogType.Warning, "[GridSystem] 检查占用超出范围: (-1, 0)");
        Assert.IsTrue(_gridSystem.IsCellOccupied(-1, 0));
        LogAssert.Expect(LogType.Warning, "[GridSystem] 检查占用超出范围: (0, -1)");
        Assert.IsTrue(_gridSystem.IsCellOccupied(0, -1));
        LogAssert.Expect(LogType.Warning, "[GridSystem] 检查占用超出范围: (5, 0)");
        Assert.IsTrue(_gridSystem.IsCellOccupied(5, 0));
        LogAssert.Expect(LogType.Warning, "[GridSystem] 检查占用超出范围: (0, 10)");
        Assert.IsTrue(_gridSystem.IsCellOccupied(0, 10));
    }

    [Test]
    public void ReleaseRow_ShouldClearAllCellsInRow()
    {
        GameObject occupant0 = new GameObject("Occupant0");
        GameObject occupant1 = new GameObject("Occupant1");
        LogAssert.Expect(LogType.Log, "[GridSystem] 格子 (2, 0) 被占用: Occupant0");
        _gridSystem.OccupyCell(2, 0, occupant0);
        LogAssert.Expect(LogType.Log, "[GridSystem] 格子 (2, 5) 被占用: Occupant1");
        _gridSystem.OccupyCell(2, 5, occupant1);

        LogAssert.Expect(LogType.Log, "[GridSystem] 已释放第 2 行所有格子");
        _gridSystem.ReleaseRow(2);

        Assert.IsFalse(_gridSystem.IsCellOccupied(2, 0));
        Assert.IsFalse(_gridSystem.IsCellOccupied(2, 5));
    }

    [Test]
    public void ReleaseRow_OutOfBounds_ShouldNotThrow()
    {
        LogAssert.Expect(LogType.Warning, "[GridSystem] 释放行超出范围: -1");
        Assert.DoesNotThrow(() => _gridSystem.ReleaseRow(-1));
        LogAssert.Expect(LogType.Warning, "[GridSystem] 释放行超出范围: 5");
        Assert.DoesNotThrow(() => _gridSystem.ReleaseRow(5));
    }

    [Test]
    public void GetOccupiedCols_ShouldReturnCorrectIndices()
    {
        GameObject occupant0 = new GameObject("Occupant0");
        GameObject occupant5 = new GameObject("Occupant5");
        LogAssert.Expect(LogType.Log, "[GridSystem] 格子 (1, 0) 被占用: Occupant0");
        _gridSystem.OccupyCell(1, 0, occupant0);
        LogAssert.Expect(LogType.Log, "[GridSystem] 格子 (1, 5) 被占用: Occupant5");
        _gridSystem.OccupyCell(1, 5, occupant5);

        var occupied = _gridSystem.GetOccupiedCols(1);

        Assert.AreEqual(2, occupied.Count);
        Assert.Contains(0, occupied);
        Assert.Contains(5, occupied);

        UnityEngine.Object.DestroyImmediate(occupant0);
        UnityEngine.Object.DestroyImmediate(occupant5);
    }

    [Test]
    public void GetOccupiedCols_EmptyRow_ShouldReturnEmptyList()
    {
        var occupied = _gridSystem.GetOccupiedCols(0);

        Assert.AreEqual(0, occupied.Count);
    }
}
