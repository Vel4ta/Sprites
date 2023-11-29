using System.Runtime.CompilerServices;
using System.Transactions;
using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.AccessControl;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.SignalR;
using System.Reflection.Metadata.Ecma335;
using webapi;
using System.Xml.Linq;

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

public class AABB
{
    public double Width { get; set; }
    public double Height { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double MaxX => X + Width;
    public double MaxY => Y + Height;
    public bool Intersects(AABB other) => MaxX > other.X && X < other.MaxX && MaxY > other.Y && Y < other.MaxY;
    public bool LessThan(AABB other) => X < other.X && Y < other.Y;
    public override string ToString()
    {
        return "Width: " + Width + " Height: " + Height + " X: " + X + " Y: " + Y + " MaxX: " + MaxX + " MaxY: " + MaxY + "\n";
    }
}

public class Node
{
    public int parent { get; set; }
    public int index { get; set; }
    public int left { get; set; }
    public int right { get; set; }
    public int height { get; set; } = 1;
    public required AABB value { get; set; }
    public bool IsLeaf => left == index && right == index;
}

public class Tree
{
    public int Root { get; set; } = 0;
    public int Height { get; set; } = 0;
    public List<Node> Nodes { get; set; } = new List<Node>();
    public int Length => Nodes.Count;

    public void Insert(AABB value)
    {
        if (this.Nodes.IsNullOrEmpty()) {
            this.Nodes.Add(new Node
            {
                parent = Root,
                index = Root,
                left = Root,
                right = Root,
                value = value
            });
            return;
        }

        int cur = Root;
        while (!Nodes[cur].IsLeaf)
        {
            if (Nodes[Nodes[cur].left].IsLeaf && Nodes[Nodes[cur].right].IsLeaf)
            {
                if ((Nodes[Nodes[cur].left].value.Width + value.Width) * (Nodes[Nodes[cur].left].value.Height + value.Height) < (Nodes[Nodes[cur].right].value.Width + value.Width) * (Nodes[Nodes[cur].right].value.Height + value.Height))
                {
                    cur = Nodes[cur].left;
                } else
                {
                    cur = Nodes[cur].right;
                }
                break;
            }

            if (!Nodes[Nodes[cur].right].IsLeaf && !Nodes[Nodes[cur].left].IsLeaf)
            {
                if (value.Intersects(Nodes[Nodes[cur].left].value))
                {
                    cur = Nodes[cur].left;
                } else 
                {
                    cur = Nodes[cur].right;
                }
                continue;
            }

            if (Nodes[Nodes[cur].left].IsLeaf && !value.Intersects(Nodes[Nodes[cur].right].value))
            {
                cur = Nodes[cur].left;
            } else 
            {
                cur = Nodes[cur].right;
            }
        }

        if (Nodes[cur].IsLeaf)
        {
            int left;
            int right;
            int p = Length + 1;
            double width, height, x, y;

            if (Root == cur)
            {
                Root = p;
            }

            if (Nodes[cur].parent != cur)
            {
                if (Nodes[Nodes[cur].parent].left == cur)
                {
                    Nodes[Nodes[cur].parent].left = p;
                } else
                {
                    Nodes[Nodes[cur].parent].right = p;
                }
            }

            bool xval = Nodes[cur].value.X < value.X;
            bool yval = Nodes[cur].value.Y < value.Y;

            if (xval)
            {
                width = value.MaxX - Nodes[cur].value.X;
                x = Nodes[cur].value.X;
            } else
            {
                width = Nodes[cur].value.MaxX - value.X;
                x = value.X;
            }

            if (yval)
            {
                height = value.MaxY - Nodes[cur].value.Y;
                y = Nodes[cur].value.Y;
            }
            else
            {
                height = Nodes[cur].value.MaxY - value.Y;
                y = value.Y;
            }

            if (xval && yval)
            {
                left = cur;
                right = Length;
                Nodes.Add(new Node
                {
                    parent = p,
                    index = right,
                    left = right,
                    right = right,
                    value = value
                });
            } else
            {
                left = Length;
                right = cur;
                Nodes.Add(new Node
                {
                    parent = p,
                    index = left,
                    left = left,
                    right = left,
                    value = value
                });
            }

            Nodes.Add(new Node
            {
                parent = Nodes[cur].parent == cur ? p : Nodes[cur].parent,
                index = p,
                left = left,
                right = right,
                value = new AABB
                {
                    Width = width,
                    Height = height,
                    X = x,
                    Y = y
                }
            });

            Nodes[cur].parent = p;
            cur = p;
        }

        /*while (cur != Root)
        {
            Console.WriteLine("parent: " + Nodes[cur].parent + Nodes[Nodes[cur].parent].value + "index: " + cur + Nodes[cur].value);
            if (Nodes[cur].value.X < Nodes[Nodes[cur].parent].value.X)
            {
                Nodes[Nodes[cur].parent].value.Width += Nodes[Nodes[cur].parent].value.X;
                Nodes[Nodes[cur].parent].value.X = Nodes[cur].value.X;
            }

            if (Nodes[cur].value.Y < Nodes[Nodes[cur].parent].value.Y)
            {
                Nodes[Nodes[cur].parent].value.Height += Nodes[Nodes[cur].parent].value.Y - Nodes[cur].value.Y;
                Nodes[Nodes[cur].parent].value.Y = Nodes[cur].value.Y;
            }

            if (Nodes[cur].value.MaxX > Nodes[Nodes[cur].parent].value.MaxX)
            {
                Nodes[Nodes[cur].parent].value.Width += Nodes[cur].value.MaxX - Nodes[Nodes[cur].parent].value.MaxX;
            }

            if (Nodes[cur].value.MaxY > Nodes[Nodes[cur].parent].value.MaxY)
            {
                Nodes[Nodes[cur].parent].value.Height += Nodes[cur].value.MaxY - Nodes[Nodes[cur].parent].value.MaxY;
            }
            cur = Nodes[cur].parent;
        }*/
        while (true)
        {
            int left = TreeHeight(cur, Nodes[cur].left);
            int right = TreeHeight(cur, Nodes[cur].right);
            Nodes[cur].height = 1 + int.Max(left, right);

            int balance = int.Max(left, right) - int.Min(left, right);
            bool sign = left < right;

            int new_root;
            if (balance > 1 && !sign && value.LessThan(Nodes[Nodes[cur].left].value))
            {
                new_root = RightRotate(cur);
            }
            else if (balance > 1 && sign && !value.LessThan(Nodes[Nodes[cur].right].value))
            {
                new_root = LeftRotate(cur);
            }
            else if (balance > 1 && !sign && !value.LessThan(Nodes[Nodes[cur].left].value))
            {
                Nodes[cur].left = LeftRotate(Nodes[cur].left);
                new_root = RightRotate(cur);
            }
            else if (balance > 1 && sign && value.LessThan(Nodes[Nodes[cur].right].value))
            {
                Nodes[cur].right = RightRotate(Nodes[cur].right);
                new_root = LeftRotate(cur);
            }
            else
            {
                new_root = Root;
            }

            if (Nodes[cur].parent == cur) {
                Root = new_root;
                break;
            }

            if (Nodes[cur].value.X < Nodes[Nodes[cur].parent].value.X)
            {
                Nodes[Nodes[cur].parent].value.Width += Nodes[Nodes[cur].parent].value.X;
                Nodes[Nodes[cur].parent].value.X = Nodes[cur].value.X;
            }

            if (Nodes[cur].value.Y < Nodes[Nodes[cur].parent].value.Y)
            {
                Nodes[Nodes[cur].parent].value.Height += Nodes[Nodes[cur].parent].value.Y - Nodes[cur].value.Y;
                Nodes[Nodes[cur].parent].value.Y = Nodes[cur].value.Y;
            }

            if (Nodes[cur].value.MaxX > Nodes[Nodes[cur].parent].value.MaxX)
            {
                Nodes[Nodes[cur].parent].value.Width += Nodes[cur].value.MaxX - Nodes[Nodes[cur].parent].value.MaxX;
            }

            if (Nodes[cur].value.MaxY > Nodes[Nodes[cur].parent].value.MaxY)
            {
                Nodes[Nodes[cur].parent].value.Height += Nodes[cur].value.MaxY - Nodes[Nodes[cur].parent].value.MaxY;
            }

            cur = Nodes[cur].parent;
        }
    }

    public int LeftRotate(int cur)
    {
        int child = Nodes[cur].right;
        int left_gc = Nodes[child].left;

        if (left_gc == child)
        {
            Nodes[cur].right = cur;
        } else
        {
            Nodes[left_gc].parent = cur;
            Nodes[cur].right = left_gc;
        }

        int p = Nodes[cur].parent;
        Nodes[cur].parent = child;
        if (p != cur)
        {
            if (Nodes[p].left == cur)
            {
                Nodes[p].left = child;
            } else
            {
                Nodes[p].right = child;
            }
        }

        if (p == cur)
        {
            Nodes[child].parent = child;
        } else
        {
            Nodes[child].parent = p;
        }
        Nodes[child].left = cur;

        Nodes[cur].height = int.Max(TreeHeight(cur, Nodes[cur].left), TreeHeight(cur, Nodes[cur].right));
        Nodes[child].height = int.Max(TreeHeight(child, Nodes[child].left), TreeHeight(child, Nodes[child].right));
        return child;
    }
    public int RightRotate(int cur)
    {
        int child = Nodes[cur].left;
        int right_gc = Nodes[child].right;

        if (right_gc == child)
        {
            Nodes[cur].left = cur;
        }
        else
        {
            Nodes[right_gc].parent = cur;
            Nodes[cur].left = right_gc;
        }

        int p = Nodes[cur].parent;
        Nodes[cur].parent = child;
        if (p != cur)
        {
            if (Nodes[p].left == cur)
            {
                Nodes[p].left = child;
            }
            else
            {
                Nodes[p].right = child;
            }
        }

        if (p == cur)
        {
            Nodes[child].parent = child;
        }
        else
        {
            Nodes[child].parent = p;
        }
        Nodes[child].right = cur;

        Nodes[cur].height = int.Max(TreeHeight(cur, Nodes[cur].left), TreeHeight(cur, Nodes[cur].right));
        Nodes[child].height = int.Max(TreeHeight(child, Nodes[child].left), TreeHeight(child, Nodes[child].right));
        return child;
    }

    public int TreeHeight(int cur, int child)
    {
        if (cur == child) return 0;

        return Nodes[child].height;
    }
}

public class Bounds
{
    public Bounds() { }
    public Bounds(params string[] args)
    {
        this.Height = Convert.ToDouble(args[0].Split("\"", 1)[0]);
        this.Width = Convert.ToDouble(args[1].Split("\"", 1)[0]);
        this.IncrimentX = Convert.ToDouble(args[2].Split("\"", 1)[0]);
        this.IncrimentY = Convert.ToDouble(args[3].Split("\"", 1)[0]);
        for (int i = 4; i < args.Length; i += 4)
        {
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
    public List<MapObject> World_Map { get; set; } = new List<MapObject>();
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
            int x = (int)(obj.X / this.IncrimentX + .5);
            int h = (int)(obj.Height / this.IncrimentY);
            int w = (int)(obj.Width / this.IncrimentX + .5);
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

    public object makeAABBTree()
    {
        Tree tree = new Tree();
        foreach (MapObject obj in this.World_Map)
        {
            tree.Insert(new AABB
            {
                Width = obj.Width,
                Height = obj.Height,
                X = obj.X,
                Y = obj.Y
            });
        }
        return new
        {
            w = (int)(Width / IncrimentX + 0.5) - 1,
            h = Height / IncrimentY - 1,
            map = tree.Nodes
        };
    }
    public override string ToString()
    {
        return "" + Height + " " + Width + " " + IncrimentX + " " + IncrimentY;
    }
}