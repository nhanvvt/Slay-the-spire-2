using Godot;

namespace MegaCrit.Sts2.addons.mega_text;

public static class ThemeConstants
{
	public static class Label
	{
		public static readonly StringName FontSize = "font_size";

		public static readonly StringName Font = "font";

		public static readonly StringName LineSpacing = "line_spacing";

		public static readonly StringName OutlineSize = "outline_size";

		public static readonly StringName FontColor = "font_color";

		public static readonly StringName FontOutlineColor = "font_outline_color";

		public static readonly StringName FontShadowColor = "font_shadow_color";
	}

	public static class RichTextLabel
	{
		public static readonly StringName NormalFont = "normal_font";

		public static readonly StringName BoldFont = "bold_font";

		public static readonly StringName ItalicsFont = "italics_font";

		public static readonly StringName LineSpacing = "line_separation";

		public static readonly StringName NormalFontSize = "normal_font_size";

		public static readonly StringName BoldFontSize = "bold_font_size";

		public static readonly StringName BoldItalicsFontSize = "bold_italics_font_size";

		public static readonly StringName ItalicsFontSize = "italics_font_size";

		public static readonly StringName MonoFontSize = "mono_font_size";

		public static readonly StringName[] AllFontSizes = new StringName[5] { NormalFontSize, BoldFontSize, BoldItalicsFontSize, ItalicsFontSize, MonoFontSize };

		public static readonly StringName DefaultColor = "default_color";

		public static readonly StringName FontOutlineColor = "font_outline_color";

		public static readonly StringName FontShadowColor = "font_shadow_color";
	}

	public static class Control
	{
		public static readonly StringName Focus = "focus";
	}

	public static class MarginContainer
	{
		public static readonly StringName MarginLeft = "margin_left";

		public static readonly StringName MarginRight = "margin_right";

		public static readonly StringName MarginTop = "margin_top";

		public static readonly StringName MarginBottom = "margin_bottom";
	}

	public static class BoxContainer
	{
		public static readonly StringName Separation = "separation";
	}

	public static class FlowContainer
	{
		public static readonly StringName HSeparation = "h_separation";

		public static readonly StringName VSeparation = "v_separation";
	}

	public static class TextEdit
	{
		public static readonly StringName Font = "font";
	}

	public static class LineEdit
	{
		public static readonly StringName Font = "font";
	}
}
