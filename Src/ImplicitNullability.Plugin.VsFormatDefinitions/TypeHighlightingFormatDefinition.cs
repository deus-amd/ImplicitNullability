using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace ImplicitNullability.Plugin.VsFormatDefinitions
{
    public static class INTypeHighlightingAttributeIds
    {
        public const string HIGHLIGHTING_ID = "INTypeHighlighting";
    }

    [ClassificationType(ClassificationTypeNames = INTypeHighlightingAttributeIds.HIGHLIGHTING_ID)]
    [Order(After = "Formal Language Priority", Before = "Natural Language Priority")]
    [Export(typeof(EditorFormatDefinition))]
    [Name(INTypeHighlightingAttributeIds.HIGHLIGHTING_ID)]
    [System.ComponentModel.DisplayName(INTypeHighlightingAttributeIds.HIGHLIGHTING_ID)]
    [UserVisible(true)]
    internal class INTypeHighlightingClassificationDefinition : ClassificationFormatDefinition
    {
        public INTypeHighlightingClassificationDefinition()
        {
            DisplayName = INTypeHighlightingAttributeIds.HIGHLIGHTING_ID;
            ForegroundColor = Colors.DeepPink;

            //TextDecoration myUnderline = new TextDecoration();

            //// Create a linear gradient pen for the text decoration.
            //Pen myPen = new Pen();
            //myPen.Brush = new LinearGradientBrush(Colors.Yellow, Colors.Red, new Point(0, 0.5), new Point(1, 0.5));
            //myPen.Brush.Opacity = 0.5;
            //myPen.Thickness = 3.5;
            //myPen.DashStyle = DashStyles.Dash;
            //myUnderline.Pen = myPen;
            ////System.Diagnostics.Debugger.Launch();
            //myUnderline.PenThicknessUnit = TextDecorationUnit.FontRecommended;

            //this.TextDecorations = new TextDecorationCollection { myUnderline };
        }

#pragma warning disable 0649
        [Export, Name(INTypeHighlightingAttributeIds.HIGHLIGHTING_ID), BaseDefinition("formal language")]
        internal ClassificationTypeDefinition ClassificationTypeDefinition;
#pragma warning restore 0649
    }
}
