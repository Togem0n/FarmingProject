using UnityEngine;

[System.Serializable]
public class GridProperty
{
    public GridCoordinate gridCoordinate;
    public GridBoolProperty gridBoolProperty;
    public GridBoolProperty[] gridBoolPropertyList;
    public bool gridBoolValue = false;

    public GridProperty(GridCoordinate gridCoordinate, GridBoolProperty gridBoolProperty, bool gridBoolValue)
    {
        this.gridCoordinate = gridCoordinate;
        this.gridBoolProperty = gridBoolProperty;
        this.gridBoolValue = gridBoolValue;
    }

    public GridProperty(GridCoordinate gridCoordinate, GridBoolProperty gridBoolProperty, GridBoolProperty[] gridBoolPropertyList, bool gridBoolValue)
    {
        this.gridCoordinate = gridCoordinate;
        this.gridBoolProperty = gridBoolProperty;
        this.gridBoolPropertyList = gridBoolPropertyList;
        this.gridBoolValue = gridBoolValue;
    }
}
