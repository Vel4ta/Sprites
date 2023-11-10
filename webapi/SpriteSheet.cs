using System.Runtime.CompilerServices;
using System.Transactions;
using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;

namespace webapi;
public class SpriteSheet
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Src { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public int Frames { get; set; }
    public int StartFrame { get; set; }
    public int Rows { get; set; }
    public int Row { get; set; }
    public double Duration { get; set; }
    public string? Event { get; set; }
    public double OffsetX => StartFrame * IncrimentX;
    public double OffsetY => Row * IncrimentY;
    public double IncrimentX => Frames > 0 ? Width / Frames : 0;
    public double IncrimentY => Rows > 0 ? Height / Rows : 0;
    public override string ToString()
    {
        return "Id: " + Id + ", Title: " + Title + ", Src: " + Src + ", Width: " + Width + ", Height: " + Height + ", Frames: " + Frames + ", Rows: " + Rows + ", Duration: " + Duration + "StartFrame: " + StartFrame + ", Row: " + Row + ", Event: " + Event;
    }
}

public class Sprite
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Src { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public int Frames { get; set; }
    public int Rows { get; set; }
    public double Duration { get; set; }

    public override string ToString()
    {
        return "Id: " + Id + ", Title: " + Title + ", Src: " + Src + ", Width: " + Width + ", Height: " + Height + ", Frames: " + Frames + ", Rows: " + Rows + ", Duration: " + Duration;
    }
}

public class SpriteSettings
{
    public int Settings_Id { get; set; }
    public int Id { get; set; }
    public int StartFrame { get; set; }
    public int Row { get; set; }
    public string? Event { get; set; }

    public override string ToString()
    {
        return "Settings_Id: " + Settings_Id + ", Id: " + Id + "StartFrame: " + StartFrame + ", Row: " + Row + ", Event: " + Event;
    }
}

public class SpriteActions : Sprite
{
    SpriteActions() : base() {  }
    public int Action_Id { get; set; }
    public int StartFrame { get; set; }
    public int Row { get; set; }
    public string? Trigger { get; set; }
}

public class SpriteIdentifier {
    public int Id { get; set; }
    public int Settings_Id { get; set; }
    public override string ToString()
    {
        return "" + Id + "" + Settings_Id;
    }
}

public class MapObject {
    public double Height { get; set; }
    public double Width { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public override string ToString()
    {
        return "" + this.Height + " " + this.Width + " " + this.X + " " + this.Y;
    }
}

public class Bounds
{
    public Bounds() { }
    public Bounds(params string[] args) {
        this.Height = Convert.ToDouble(args[0].Split("\"", 1)[0]);
        this.Width = Convert.ToDouble(args[1].Split("\"", 1)[0]);
        this.IncrimentX = Convert.ToDouble(args[2].Split("\"", 1)[0]);
        this.IncrimentY = Convert.ToDouble(args[3].Split("\"", 1)[0]);
        for (int i = 4; i < args.Length; i += 4) {
            this.World_Map.Add(new MapObject
            {
                Height = Convert.ToDouble(args[i].Split("\"", 1)[0]),
                Width = Convert.ToDouble(args[i + 1].Split("\"", 1)[0]),
                X = Convert.ToDouble(args[i + 2].Split("\"", 1)[0]),
                Y = Convert.ToDouble(args[i + 3].Split("\"", 1)[0])
            });
        }
    }
    public double Height { get; set; }
    public double Width { get; set; }
    public double IncrimentX { get; set; }
    public double IncrimentY { get; set; }
    public List<MapObject> World_Map {  get; set; } = new List<MapObject>();
    public object makeBounds()
    {
        int[][] wm = new int[(int)(this.Height / this.IncrimentY)][];
        for (int i = 0; i < wm.Length; i++)
        {
            wm[i] = new int[(int)(this.Width / this.IncrimentX + .5)];
        }
        foreach (MapObject obj in this.World_Map)
        {
            int y = (int)(obj.Y / this.IncrimentY);
            int x = (int)(obj.X / this.IncrimentX);
            int h = (int)(obj.Height / this.IncrimentY);
            int w = (int)(obj.Width / this.IncrimentX);
            Console.WriteLine(this.IncrimentX.ToString() + " " + this.IncrimentY.ToString());
            Console.WriteLine(obj.ToString());
            Console.WriteLine(h.ToString() + " " + w.ToString() + " " + x.ToString() + " " + y.ToString());
            for (int i = y + (h > 0 ? 1 : h); i <= y + h; i++)
            {
                if (i <= y + 1 || i == y + h)
                {
                    for (int j = x; j < x + (w > 0 ? w : 1); j++)
                    {
                        wm[i + h][j] = 1;
                    }
                    continue;
                }

                wm[i + h][x] = 1;
                wm[i + h][x + w] = 1;
            }
        }

        return new
        {
            w = (int)(this.Width / this.IncrimentX + 0.5) - 1,
            h = wm.Length - 1,
            map = wm,
        };
    }
    public override string ToString()
    {
        return "" + Height + " " + Width + " " + IncrimentX + " " + IncrimentY;
    }
}

public class Request
{
    public string? Operation { get; set; }
    public object[]? Parameters { get; set; }
    public string[]? Args => this.Parameters.Select(x => x.ToString()).ToArray();
}

public class Option<T> where T : class
{
    public T? Value { get; set; }
}
