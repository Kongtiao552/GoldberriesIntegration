using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GoldberriesStat.Misc;

public class TableHelper {
    
    public class TableCell {
        
        public float Width { get; set; }
        public float Height { get; set; }

        private string _Text;

        public string Text {

            get {
                return _Text;
            }

            // Truncate the string if needed

            set {
                StringBuilder builder = new StringBuilder();
                float width = 0f;

                for (int i = 0; i < value.Length; i++) {
                    char c = value[i];
                    width += ActiveFont.Measure(c).X * TextSize;

                    if (width > Width) {
                        builder.Remove(i - 4, 4);
                        builder.Append(" ...");
                        break;
                    }

                    builder.Append(c);
                }

                _Text = builder.ToString();
            }

        }

        public Color TextColor { get; set; }
        public Color? BgColor { get; set; }
        public float TextSize { get; set; }
        public bool TextOutline { get; set; } = false;
        public float TextOutlineSize { get; set; } = 1f;
        public Color TextOutlineColor { get; set; } = Color.Black;

    }

    public string Header { get; set; }
    public float HeaderSize { get; set; }

    protected List<List<TableCell>> TableCells { get; set; } = new List<List<TableCell>>();

    public TableCell this[int x, int y] {
        get => TableCells[x][y];
        set => TableCells[x][y] = value;
    }

    public void Insert(int x, int y, TableCell cell) {
        while (TableCells.Count <= x) {
            TableCells.Add(new List<TableCell>());
        }

        TableCells[x].Insert(y, cell);
    }

    public void Insert(int x, int y, float width, float height, string text, float textSize, Color? textColor = null, Color? color = null, bool outline = false, float outlineSize = 1f, Color? outlineColor = null) {
        TableCell cell = new TableCell();
        cell.Width = width;
        cell.Height = height;
        cell.TextColor = textColor ?? Color.Black;
        cell.TextOutline = outline;
        cell.TextOutlineSize = outlineSize;
        cell.TextOutlineColor = outlineColor ?? Color.Black;
        cell.TextSize = textSize;
        cell.Text = text;
        cell.BgColor = color;

        Insert(x, y, cell);
    }

    public void DrawTable(Vector2 pointer) {
        Vector2 orig = pointer;
        foreach (List<TableCell> column in TableCells) {
            foreach (TableCell cell in column) {
                if (cell == null) {
                    break;
                }

                if (cell.BgColor != null) {
                    Draw.Rect(pointer, cell.Width, cell.Height, cell.BgColor.Value);
                }

                if (cell.TextOutline) {
                    DrawCell(cell.Text, pointer, cell.TextSize, cell.Width, cell.Height, 2f, cell.TextColor, Color.Black, cell.TextOutlineSize, cell.TextOutlineColor);   
                } else {
                    DrawCell(cell.Text, pointer, cell.TextSize, cell.Width, cell.Height, 2f, cell.TextColor, Color.Black);
                }
                    

                pointer.Y += cell.Height;
            }

            pointer.Y = orig.Y;
            pointer.X += column[0].Width;
        }
    }

    public static void DrawCell(string text, Vector2 pointer, float textSize, float width, float height) {
        DrawCell(text, pointer, textSize, width, height, 2f, Color.Black, Color.Black);
    }

    public static void DrawCellOutline(Vector2 pointer, float width, float height, float outlineThinkness, Color outlineColor) {
        Draw.Rect(pointer, width + outlineThinkness, outlineThinkness, outlineColor);
        Draw.Rect(pointer, outlineThinkness, height + outlineThinkness, outlineColor);
        Draw.Rect(pointer + new Vector2(width, 0f), outlineThinkness, height + outlineThinkness, outlineColor);
        Draw.Rect(pointer + new Vector2(0f, height), width + outlineThinkness, outlineThinkness, outlineColor);
    }

    public static void DrawCell(string text, Vector2 pointer, float textSize, float width, float height, float outlineThinkness, Color textColor, Color outlineColor) {
        DrawCellOutline(pointer, width, height, outlineThinkness, outlineColor);
        pointer += new Vector2(width, height) / 2;
        ActiveFont.Draw(
            text,
            pointer,
            Vector2.One / 2,
            Vector2.One * textSize,
            textColor
        );
    }

    public static void DrawCell(string text, Vector2 pointer, float textSize, float width, float height, float outlineThinkness, Color textColor, Color outlineColor, float textOutlineSize, Color textOutlineColor) {
        DrawCellOutline(pointer, width, height, outlineThinkness, outlineColor);
        pointer += new Vector2(width, height) / 2;
        ActiveFont.DrawOutline(
            text,
            pointer,
            Vector2.One / 2,
            Vector2.One * textSize,
            textColor,
            textOutlineSize,
            textOutlineColor
        );
    }


}