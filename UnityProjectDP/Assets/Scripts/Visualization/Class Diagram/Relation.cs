using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Relation
{
    private string connectorXmiId;

    private string sourceXmiId;
    //inside source
    private int sourceModelEaLocalId;
    private string sourceModelType;
    private string sourceModelName;
    private string sourceRoleVisibility;
    private string sourceRoleTargetScope;
    private string sourceTypeAggregation;
    private string sourceTypeContainment;
    private bool sourceModifiersIsOrdered;
    private string sourceModifiersChangeable;
    private bool sourceModifiersisNavigablee;
    private string sourceStyleValue;
    private string sourceName;


    private string targetXmiId;
    //inside target
    private int targetModelEaLocalId;
    private string targetModelType;
    private string targetModelName;
    private string targetRoleVisibility;
    private string targetRoleTargetScope;
    private string targetTypeAggregation;
    private string targetTypeContainment;
    private string targetName;
    private bool targetModifiersIsOrdered;
    private string targetModifiersChangeable;
    private bool targetModifiersisNavigablee;
    private string targetStyleValue;


    private int modelEaLocalId;

    private string propertiesEa_type;
    private string properitesDirection;

    private bool modifiersIsRoot;
    private bool modifiersIsLeaf;

    private string appearanceLinemode;
    private string appearanceLineColor;
    private string appearanceLinewidth;
    private string appearanceSeqno;
    private string appearanceHeadStyle;
    private string appearanceLineStyle;

    private string extendedPropertiesVirtualInheritance;

    private string sourceMultiplicity;
    private string targetMultiplicity;
    private string label;

    public string ConnectorXmiId { get => connectorXmiId; set => connectorXmiId = value; }
    public string SourceXmiId { get => sourceXmiId; set => sourceXmiId = value; }
    public int SourceModelEaLocalId { get => sourceModelEaLocalId; set => sourceModelEaLocalId = value; }
    public string SourceModelType { get => sourceModelType; set => sourceModelType = value; }
    public string SourceModelName { get => sourceModelName; set => sourceModelName = value; }
    public string SourceRoleVisibility { get => sourceRoleVisibility; set => sourceRoleVisibility = value; }
    public string SourceRoleTargetScope { get => sourceRoleTargetScope; set => sourceRoleTargetScope = value; }
    public string SourceTypeAggregation { get => sourceTypeAggregation; set => sourceTypeAggregation = value; }
    public string SourceTypeContainment { get => sourceTypeContainment; set => sourceTypeContainment = value; }
    public bool SourceModifiersIsOrdered { get => sourceModifiersIsOrdered; set => sourceModifiersIsOrdered = value; }
    public string SourceModifiersChangeable { get => sourceModifiersChangeable; set => sourceModifiersChangeable = value; }
    public bool SourceModifiersisNavigablee { get => sourceModifiersisNavigablee; set => sourceModifiersisNavigablee = value; }
    public string SourceStyleValue { get => sourceStyleValue; set => sourceStyleValue = value; }
    public string SourceName { get => sourceName; set => sourceName = value; }
    public string TargetXmiId { get => targetXmiId; set => targetXmiId = value; }
    public int TargetModelEaLocalId { get => targetModelEaLocalId; set => targetModelEaLocalId = value; }
    public string TargetModelType { get => targetModelType; set => targetModelType = value; }
    public string TargetModelName { get => targetModelName; set => targetModelName = value; }
    public string TargetRoleVisibility { get => targetRoleVisibility; set => targetRoleVisibility = value; }
    public string TargetRoleTargetScope { get => targetRoleTargetScope; set => targetRoleTargetScope = value; }
    public string TargetTypeAggregation { get => targetTypeAggregation; set => targetTypeAggregation = value; }
    public string TargetTypeContainment { get => targetTypeContainment; set => targetTypeContainment = value; }
    public string TargetName { get => targetName; set => targetName = value; }
    public bool TargetModifiersIsOrdered { get => targetModifiersIsOrdered; set => targetModifiersIsOrdered = value; }
    public string TargetModifiersChangeable { get => targetModifiersChangeable; set => targetModifiersChangeable = value; }
    public bool TargetModifiersisNavigablee { get => targetModifiersisNavigablee; set => targetModifiersisNavigablee = value; }
    public string TargetStyleValue { get => targetStyleValue; set => targetStyleValue = value; }
    public int ModelEaLocalId { get => modelEaLocalId; set => modelEaLocalId = value; }
    public string PropertiesEa_type { get => propertiesEa_type; set => propertiesEa_type = value; }
    public string ProperitesDirection { get => properitesDirection; set => properitesDirection = value; }
    public bool ModifiersIsRoot { get => modifiersIsRoot; set => modifiersIsRoot = value; }
    public bool ModifiersIsLeaf { get => modifiersIsLeaf; set => modifiersIsLeaf = value; }
    public string AppearanceLinemode { get => appearanceLinemode; set => appearanceLinemode = value; }
    public string AppearanceLineColor { get => appearanceLineColor; set => appearanceLineColor = value; }
    public string AppearanceLinewidth { get => appearanceLinewidth; set => appearanceLinewidth = value; }
    public string AppearanceSeqno { get => appearanceSeqno; set => appearanceSeqno = value; }
    public string AppearanceHeadStyle { get => appearanceHeadStyle; set => appearanceHeadStyle = value; }
    public string AppearanceLineStyle { get => appearanceLineStyle; set => appearanceLineStyle = value; }
    public string ExtendedPropertiesVirtualInheritance { get => extendedPropertiesVirtualInheritance; set => extendedPropertiesVirtualInheritance = value; }



    public GameObject PrefabType { get; set; }
    public string FromClass { get; set; }
    public string ToClass { get; set; }
    public string SourceMultiplicity { get => sourceMultiplicity; set => sourceMultiplicity = value; }
    public string TargetMultiplicity { get => targetMultiplicity; set => targetMultiplicity = value; }
    public string Label { get => label; set => label = value; }

    public string OALName;

    public Relation(string fromClassId, string toClassId)
    {
        this.SourceModelName = fromClassId;
        this.TargetModelName = toClassId;
        PrefabType = null;
        this.OALName = "Rx";
    }
    public Relation(string fromClassId, string toClassId, GameObject prefabType)
    {
        this.SourceModelName = fromClassId;
        this.TargetModelName = toClassId;
        this.PrefabType = prefabType;
        this.OALName = "Rx";
    }
    public Relation() { }
    public void DebugProperties(Relation rel)
    {
        //PropertyInfo[] property = rel.GetProperties();
    } 
}
