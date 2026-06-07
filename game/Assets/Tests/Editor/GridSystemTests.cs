using NUnit.Framework;
using UnityEngine;

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
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
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

        bool result = _gridSystem.OccupyCell(0, 0, occupant);

        Assert.IsTrue(result);
        Assert.IsTrue(_gridSystem.IsCellOccupied(0, 0));

        Object.DestroyImmediate(occupant);
    }

    [Test]
    public void OccupyCell_OccupiedCell_ShouldReturnFalse()
    {
        GameObject occupant1 = new GameObject("Occupant1");
        GameObject occupant2 = new GameObject("Occupant2");

        _gridSystem.OccupyCell(0, 0, occupant1);
        bool result = _gridSystem.OccupyCell(0, 0, occupant2);

        Assert.IsFalse(result);

        Object.DestroyImmediate(occupant1);
        Object.DestroyImmediate(occupant2);
    }

    [Test]
    public void OccupyCell_OutOfBounds_ShouldReturnFalse()
    {
        GameObject occupant = new GameObject("TestOccupant");

        bool result = _gridSystem.OccupyCell(-1, 0, occupant);

        Assert.IsFalse(result);

        Object.DestroyImmediate(occupant);
    }

    [Test]
    public void ReleaseCell_OccupiedCell_ShouldClearOccupation()
    {
        GameObject occupant = new GameObject("TestOccupant");
        _gridSystem.OccupyCell(1, 1, occupant);

        _gridSystem.ReleaseCell(1, 1);

        Assert.IsFalse(_gridSystem.IsCellOccupied(1, 1));

        Object.DestroyImmediate(occupant);
    }

    [Test]
    public void IsCellOccupied_OutOfBounds_ShouldReturnTrue()
    {
        Assert.IsTrue(_gridSystem.IsCellOccupied(-1, 0));
        Assert.IsTrue(_gridSystem.IsCellOccupied(0, -1));
        Assert.IsTrue(_gridSystem.IsCellOccupied(5, 0));
        Assert.IsTrue(_gridSystem.IsCellOccupied(0, 10));
    }

    [Test]
    public void ReleaseRow_ShouldClearAllCellsInRow()
    {
        GameObject occupant0 = new GameObject("Occupant0");
        GameObject occupant1 = new GameObject("Occupant1");
        _gridSystem.OccupyCell(2, 0, occupant0);
        _gridSystem.OccupyCell(2, 5, occupant1);

        _gridSystem.ReleaseRow(2);

        Assert.IsFalse(_gridSystem.IsCellOccupied(2, 0));
        Assert.IsFalse(_gridSystem.IsCellOccupied(2, 5));
    }

    [Test]
    public void ReleaseRow_OutOfBounds_ShouldNotThrow()
    {
        Assert.DoesNotThrow(() => _gridSystem.ReleaseRow(-1));
        Assert.DoesNotThrow(() => _gridSystem.ReleaseRow(5));
    }

    [Test]
    public void GetOccupiedCols_ShouldReturnCorrectIndices()
    {
        GameObject occupant0 = new GameObject("Occupant0");
        GameObject occupant5 = new GameObject("Occupant5");
        _gridSystem.OccupyCell(1, 0, occupant0);
        _gridSystem.OccupyCell(1, 5, occupant5);

        var occupied = _gridSystem.GetOccupiedCols(1);

        Assert.AreEqual(2, occupied.Count);
        Assert.Contains(0, occupied);
        Assert.Contains(5, occupied);

        Object.DestroyImmediate(occupant0);
        Object.DestroyImmediate(occupant5);
    }

    [Test]
    public void GetOccupiedCols_EmptyRow_ShouldReturnEmptyList()
    {
        var occupied = _gridSystem.GetOccupiedCols(0);

        Assert.AreEqual(0, occupied.Count);
    }
}
