using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.Features2D;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.CV.UI;
using System.Windows.Forms;

namespace SymbolsSegmentationT
{
    public class Way
    {
    //    public List<Point> points;
    //    public double accum;
    //    public Image<Gray, Byte> image;
    //    public double coeffDown;
    //    public double coeffLeftRight;
    //    public int startX;

    //    private List<double> nextPointsValues;
    //    public List<Point> auxPoints;

    //    public Way(Image<Gray, Byte> img, double coefficientDown, double coefficientLeftRight, int startX)
    //    {
    //        image = img;
    //        coeffDown = coefficientDown;
    //        coeffLeftRight = coefficientLeftRight;
    //        this.startX = startX;
    //        points = new List<Point>();
    //        accum = 0;

    //        nextPointsValues = new List<double>(5);
    //        auxPoints = new List<Point>(5);

    //        for (int i = 0; i < 5; i++)
    //        {
    //            nextPointsValues.Add(0);
    //            auxPoints.Add(new Point(0, 0));
    //        }
    //    }

    //    public void BuildPath()
    //    {
    //        int y = 0, x = startX;
    //        Point p;

    //        p = new Point(x,y);
    //        points.Add(p);
            
    //        while (p.Y < image.Height - 1)
    //        {
    //            p = FindNextPoint(p.X,p.Y);
    //            if (points.Count > 2 && p == points[points.Count - 2])
    //                coeffLeftRight += 0.5;
    //            points.Add(p);
    //        }
    //    }

    //    private Point FindNextPoint(int x, int y)
    //    {
    //        if (x - 1 < 0) x++;
    //        if (x + 1 > image.Width) x--;

    //        this.auxPoints[0] = new Point { X = x - 1, Y = y };
    //        this.auxPoints[1] = new Point { X = x - 1, Y = y + 1 };
    //        this.auxPoints[2] = new Point { X = x, Y = y + 1 };
    //        this.auxPoints[3] = new Point { X = x + 1, Y = y + 1 };
    //        this.auxPoints[4] = new Point { X = x + 1, Y = y };

    //        nextPointsValues[0] = coeffLeftRight * (double)image.Data[y, x - 1, 0];
    //        nextPointsValues[1] = coeffDown * (double)image.Data[y + 1, x - 1,0];
    //        nextPointsValues[2] = image.Data[y + 1, x, 0];
    //        nextPointsValues[3] = coeffDown * (double)image.Data[y + 1, x + 1, 0];
    //        nextPointsValues[4] = coeffLeftRight * (double)image.Data[y, x + 1, 0];

    //        int ind;
    //        ind = nextPointsValues.FindIndex(a => a == nextPointsValues.Min());
    //        accum += nextPointsValues[ind];

    //        return auxPoints[ind];
    //    }

    //    public void AstarImpl()
    //    {
    //        Random rnd = new Random();
    //        MyPathNode[,] grid = new MyPathNode[image.Width, image.Height + 1];

    //        // setup grid with walls
    //        for (int x = 0; x < image.Width; x++)
    //        {
    //            for (int y = 0; y < image.Height + 1; y++)
    //            {
    //                grid[x, y] = new MyPathNode()
    //                {
    //                    IsWall = false,
    //                    X = x,
    //                    Y = y,
    //                };
    //            }
    //        }
            
    //        // compute and display path
    //        MySolver<MyPathNode, Object> aStar = new MySolver<MyPathNode, Object>(grid);
    //        aStar.image = image;
    //        aStar.coeffDown = coeffDown;
    //        IEnumerable<MyPathNode> path;

    //        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

    //        watch.Start();
    //        {
    //            path = aStar.Search(new Point(startX, 0), new Point(startX, image.Height-1), null);
    //        }
    //        watch.Stop();

    //        //MessageBox.Show("Pathfinding took " + watch.ElapsedMilliseconds + "ms to complete.");

    //        foreach (MyPathNode node in path)
    //        {
    //            if (node.Y == image.Height) break;
    //            points.Add(new Point(node.X, node.Y));
    //        }
    //    }
    //}


    //public class MySolver<TPathNode, TUserContext> : Astar.SpatialAStar<TPathNode, TUserContext> where TPathNode : Astar.IPathNode<TUserContext>
    //{
    //    public Image<Gray, Byte> image;
    //    public double coeffDown;

    //    protected override Double Heuristic(PathNode inStart, PathNode inEnd)
    //    {
    //        return 0;// Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);
    //    }

    //    protected override Double NeighborDistance(PathNode inStart, PathNode inEnd)
    //    {
    //        if (inEnd.Y == image.Height) 
    //        {
    //            return 0;
    //        }


    //        if (inStart.X == inEnd.X)
    //        {
    //            return image.Data[inEnd.Y, inEnd.X, 0];
    //        }
    //        else
    //        {
    //            return coeffDown * (double)image.Data[inEnd.Y, inEnd.X, 0] + 0;
    //        }
    //    }

    //    public MySolver(TPathNode[,] inGrid)
    //        : base(inGrid)
    //    {
    //    }
    //}
    
    //public class MyPathNode : Astar.IPathNode<Object>
    //{
    //    public Int32 X { get; set; }
    //    public Int32 Y { get; set; }
    //    public Boolean IsWall { get; set; }

    //    public bool IsWalkable(Object unused)
    //    {
    //        return !IsWall;
    //    }
    }
}
