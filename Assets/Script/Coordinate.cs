using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinate
{
    public int X { get; set; }
    public int Y { get; set; }

    public Coordinate(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Coordinate operator+(Coordinate p1, Coordinate p2)
    {
        return new Coordinate(p1.X + p2.X, p1.Y + p2.Y);
    }

    public static Coordinate operator -(Coordinate p1, Coordinate p2)
    {
        return new Coordinate(p1.X - p2.X, p1.Y - p2.Y);
    }
}