﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=4.0.30319.33440.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlRootAttribute("mapData", Namespace="", IsNullable=false)]
public partial class XmlMapData {
    
    private XmlParameters parametersField;
    
    private XmlResolution[] resolutionListField;
    
    private XmlLookList lookListField;
    
    private XmlElementList elementListField;
    
    /// <remarks/>
    public XmlParameters parameters {
        get {
            return this.parametersField;
        }
        set {
            this.parametersField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("resolution", IsNullable=false)]
    public XmlResolution[] resolutionList {
        get {
            return this.resolutionListField;
        }
        set {
            this.resolutionListField = value;
        }
    }
    
    /// <remarks/>
    public XmlLookList lookList {
        get {
            return this.lookListField;
        }
        set {
            this.lookListField = value;
        }
    }
    
    /// <remarks/>
    public XmlElementList elementList {
        get {
            return this.elementListField;
        }
        set {
            this.elementListField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class XmlParameters {
    
    private XmlLanguage[] languageListField;
    
    private XmlName[] mapNameField;
    
    private string mapIdField;
    
    private string logFileNameField;
    
    private XmlLogLevelEnum fileLogLevelField;
    
    private XmlLogLevelEnum consoleLogLevelField;
    
    private string databaseNameField;
    
    private string kmlDirField;
    
    private double maxConnectionDistanceInKmField;
    
    private double superposingThresholdField;
    
    private XmlProjectionEnum projectionField;
    
    private double zoomMinDistanceField;
    
    private double zoomMaxDistanceField;
    
    private double resolutionThresholdField;
    
    private double sizeParameter1Field;
    
    private double sizeParameter2Field;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("language", IsNullable=false)]
    public XmlLanguage[] languageList {
        get {
            return this.languageListField;
        }
        set {
            this.languageListField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("name", IsNullable=false)]
    public XmlName[] mapName {
        get {
            return this.mapNameField;
        }
        set {
            this.mapNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
    public string mapId {
        get {
            return this.mapIdField;
        }
        set {
            this.mapIdField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string logFileName {
        get {
            return this.logFileNameField;
        }
        set {
            this.logFileNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public XmlLogLevelEnum fileLogLevel {
        get {
            return this.fileLogLevelField;
        }
        set {
            this.fileLogLevelField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public XmlLogLevelEnum consoleLogLevel {
        get {
            return this.consoleLogLevelField;
        }
        set {
            this.consoleLogLevelField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string databaseName {
        get {
            return this.databaseNameField;
        }
        set {
            this.databaseNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string kmlDir {
        get {
            return this.kmlDirField;
        }
        set {
            this.kmlDirField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double maxConnectionDistanceInKm {
        get {
            return this.maxConnectionDistanceInKmField;
        }
        set {
            this.maxConnectionDistanceInKmField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double superposingThreshold {
        get {
            return this.superposingThresholdField;
        }
        set {
            this.superposingThresholdField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public XmlProjectionEnum projection {
        get {
            return this.projectionField;
        }
        set {
            this.projectionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double zoomMinDistance {
        get {
            return this.zoomMinDistanceField;
        }
        set {
            this.zoomMinDistanceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double zoomMaxDistance {
        get {
            return this.zoomMaxDistanceField;
        }
        set {
            this.zoomMaxDistanceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double resolutionThreshold {
        get {
            return this.resolutionThresholdField;
        }
        set {
            this.resolutionThresholdField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double sizeParameter1 {
        get {
            return this.sizeParameter1Field;
        }
        set {
            this.sizeParameter1Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double sizeParameter2 {
        get {
            return this.sizeParameter2Field;
        }
        set {
            this.sizeParameter2Field = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class XmlLanguage {
    
    private XmlLanguageId idField;
    
    private string nameField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public XmlLanguageId id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
public enum XmlLanguageId {
    
    /// <remarks/>
    fr,
    
    /// <remarks/>
    en,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class XmlPointElement {
    
    private XmlName[] nameField;
    
    private XmlName[] shortNameField;
    
    private string idField;
    
    private string lookField;
    
    private double importanceField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("name", IsNullable=false)]
    public XmlName[] name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("name", IsNullable=false)]
    public XmlName[] shortName {
        get {
            return this.shortNameField;
        }
        set {
            this.shortNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="IDREF")]
    public string look {
        get {
            return this.lookField;
        }
        set {
            this.lookField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double importance {
        get {
            return this.importanceField;
        }
        set {
            this.importanceField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class XmlName {
    
    private XmlLanguageIdRef languageField;
    
    private string textField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public XmlLanguageIdRef language {
        get {
            return this.languageField;
        }
        set {
            this.languageField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string text {
        get {
            return this.textField;
        }
        set {
            this.textField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
public enum XmlLanguageIdRef {
    
    /// <remarks/>
    fr,
    
    /// <remarks/>
    en,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class XmlAttachedElement {
    
    private string idField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="IDREF")]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class XmlLineElement {
    
    private XmlName[] nameField;
    
    private XmlName[] shortNameField;
    
    private XmlAttachedElement[] attachmentListField;
    
    private string idField;
    
    private string lookField;
    
    private double importanceField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("name", IsNullable=false)]
    public XmlName[] name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("name", IsNullable=false)]
    public XmlName[] shortName {
        get {
            return this.shortNameField;
        }
        set {
            this.shortNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("element", IsNullable=false)]
    public XmlAttachedElement[] attachmentList {
        get {
            return this.attachmentListField;
        }
        set {
            this.attachmentListField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="IDREF")]
    public string look {
        get {
            return this.lookField;
        }
        set {
            this.lookField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double importance {
        get {
            return this.importanceField;
        }
        set {
            this.importanceField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class XmlCoveredElement {
    
    private string idField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="IDREF")]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class XmlPolygonElement {
    
    private XmlName[] nameField;
    
    private XmlName[] shortNameField;
    
    private XmlCoveredElement[] coveredElementListField;
    
    private string idField;
    
    private string lookField;
    
    private double importanceField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("name", IsNullable=false)]
    public XmlName[] name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("name", IsNullable=false)]
    public XmlName[] shortName {
        get {
            return this.shortNameField;
        }
        set {
            this.shortNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("element", IsNullable=false)]
    public XmlCoveredElement[] coveredElementList {
        get {
            return this.coveredElementListField;
        }
        set {
            this.coveredElementListField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="IDREF")]
    public string look {
        get {
            return this.lookField;
        }
        set {
            this.lookField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double importance {
        get {
            return this.importanceField;
        }
        set {
            this.importanceField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class XmlElementList {
    
    private XmlPolygonElement[] polygonElementListField;
    
    private XmlLineElement[] lineElementListField;
    
    private XmlPointElement[] pointElementListField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("polygonElement", IsNullable=false)]
    public XmlPolygonElement[] polygonElementList {
        get {
            return this.polygonElementListField;
        }
        set {
            this.polygonElementListField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("lineElement", IsNullable=false)]
    public XmlLineElement[] lineElementList {
        get {
            return this.lineElementListField;
        }
        set {
            this.lineElementListField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("pointElement", IsNullable=false)]
    public XmlPointElement[] pointElementList {
        get {
            return this.pointElementListField;
        }
        set {
            this.pointElementListField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class XmlPointLook {
    
    private string idField;
    
    private int pointZIndexField;
    
    private byte pointAlphaField;
    
    private byte pointRedField;
    
    private byte pointGreenField;
    
    private byte pointBlueField;
    
    private double pointSizeField;
    
    private byte textAlphaField;
    
    private byte textRedField;
    
    private byte textGreenField;
    
    private byte textBlueField;
    
    private double textSizeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int pointZIndex {
        get {
            return this.pointZIndexField;
        }
        set {
            this.pointZIndexField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte pointAlpha {
        get {
            return this.pointAlphaField;
        }
        set {
            this.pointAlphaField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte pointRed {
        get {
            return this.pointRedField;
        }
        set {
            this.pointRedField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte pointGreen {
        get {
            return this.pointGreenField;
        }
        set {
            this.pointGreenField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte pointBlue {
        get {
            return this.pointBlueField;
        }
        set {
            this.pointBlueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double pointSize {
        get {
            return this.pointSizeField;
        }
        set {
            this.pointSizeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte textAlpha {
        get {
            return this.textAlphaField;
        }
        set {
            this.textAlphaField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte textRed {
        get {
            return this.textRedField;
        }
        set {
            this.textRedField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte textGreen {
        get {
            return this.textGreenField;
        }
        set {
            this.textGreenField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte textBlue {
        get {
            return this.textBlueField;
        }
        set {
            this.textBlueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double textSize {
        get {
            return this.textSizeField;
        }
        set {
            this.textSizeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class XmlLineLook {
    
    private string idField;
    
    private int lineZIndexField;
    
    private byte lineAlphaField;
    
    private byte lineRedField;
    
    private byte lineGreenField;
    
    private byte lineBlueField;
    
    private double lineSizeField;
    
    private byte textAlphaField;
    
    private byte textRedField;
    
    private byte textGreenField;
    
    private byte textBlueField;
    
    private double textSizeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int lineZIndex {
        get {
            return this.lineZIndexField;
        }
        set {
            this.lineZIndexField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte lineAlpha {
        get {
            return this.lineAlphaField;
        }
        set {
            this.lineAlphaField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte lineRed {
        get {
            return this.lineRedField;
        }
        set {
            this.lineRedField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte lineGreen {
        get {
            return this.lineGreenField;
        }
        set {
            this.lineGreenField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte lineBlue {
        get {
            return this.lineBlueField;
        }
        set {
            this.lineBlueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double lineSize {
        get {
            return this.lineSizeField;
        }
        set {
            this.lineSizeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte textAlpha {
        get {
            return this.textAlphaField;
        }
        set {
            this.textAlphaField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte textRed {
        get {
            return this.textRedField;
        }
        set {
            this.textRedField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte textGreen {
        get {
            return this.textGreenField;
        }
        set {
            this.textGreenField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte textBlue {
        get {
            return this.textBlueField;
        }
        set {
            this.textBlueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double textSize {
        get {
            return this.textSizeField;
        }
        set {
            this.textSizeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class XmlPolygonLook {
    
    private string idField;
    
    private int contourZIndexField;
    
    private byte contourAlphaField;
    
    private byte contourRedField;
    
    private byte contourGreenField;
    
    private byte contourBlueField;
    
    private double contourSizeField;
    
    private double fillLightRatioField;
    
    private int fillZIndexField;
    
    private byte textAlphaField;
    
    private byte textRedField;
    
    private byte textGreenField;
    
    private byte textBlueField;
    
    private double textSizeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int contourZIndex {
        get {
            return this.contourZIndexField;
        }
        set {
            this.contourZIndexField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte contourAlpha {
        get {
            return this.contourAlphaField;
        }
        set {
            this.contourAlphaField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte contourRed {
        get {
            return this.contourRedField;
        }
        set {
            this.contourRedField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte contourGreen {
        get {
            return this.contourGreenField;
        }
        set {
            this.contourGreenField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte contourBlue {
        get {
            return this.contourBlueField;
        }
        set {
            this.contourBlueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double contourSize {
        get {
            return this.contourSizeField;
        }
        set {
            this.contourSizeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double fillLightRatio {
        get {
            return this.fillLightRatioField;
        }
        set {
            this.fillLightRatioField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int fillZIndex {
        get {
            return this.fillZIndexField;
        }
        set {
            this.fillZIndexField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte textAlpha {
        get {
            return this.textAlphaField;
        }
        set {
            this.textAlphaField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte textRed {
        get {
            return this.textRedField;
        }
        set {
            this.textRedField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte textGreen {
        get {
            return this.textGreenField;
        }
        set {
            this.textGreenField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte textBlue {
        get {
            return this.textBlueField;
        }
        set {
            this.textBlueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double textSize {
        get {
            return this.textSizeField;
        }
        set {
            this.textSizeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class XmlLookList {
    
    private XmlPolygonLook[] polygonLookListField;
    
    private XmlLineLook[] lineLookListField;
    
    private XmlPointLook[] pointLookListField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("polygonLook", IsNullable=false)]
    public XmlPolygonLook[] polygonLookList {
        get {
            return this.polygonLookListField;
        }
        set {
            this.polygonLookListField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("lineLook", IsNullable=false)]
    public XmlLineLook[] lineLookList {
        get {
            return this.lineLookListField;
        }
        set {
            this.lineLookListField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("pointLook", IsNullable=false)]
    public XmlPointLook[] pointLookList {
        get {
            return this.pointLookListField;
        }
        set {
            this.pointLookListField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class XmlResolution {
    
    private string outputDirField;
    
    private double sampleLength1Field;
    
    private string sampleRatioField;
    
    private string smoothingWidthField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string outputDir {
        get {
            return this.outputDirField;
        }
        set {
            this.outputDirField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double sampleLength1 {
        get {
            return this.sampleLength1Field;
        }
        set {
            this.sampleLength1Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string sampleRatio {
        get {
            return this.sampleRatioField;
        }
        set {
            this.sampleRatioField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string smoothingWidth {
        get {
            return this.smoothingWidthField;
        }
        set {
            this.smoothingWidthField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
public enum XmlLogLevelEnum {
    
    /// <remarks/>
    NO_LOG,
    
    /// <remarks/>
    ERROR,
    
    /// <remarks/>
    WARNING,
    
    /// <remarks/>
    MESSAGE,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
public enum XmlProjectionEnum {
    
    /// <remarks/>
    ORTHOGRAPHIC,
    
    /// <remarks/>
    MERCATOR,
}
