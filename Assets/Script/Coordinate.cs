using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Coordinate : INetworkSerializable
{
    public int X;
    public int Y;

    public Coordinate(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref X);
        serializer.SerializeValue(ref Y);
    }

    public static Coordinate operator+(Coordinate p1, Coordinate p2)
    {
        return new Coordinate(p1.X + p2.X, p1.Y + p2.Y);
    }

    public static Coordinate operator-(Coordinate p1, Coordinate p2)
    {
        return new Coordinate(p1.X - p2.X, p1.Y - p2.Y);
    }

    public static Coordinate operator*(Coordinate p1, int num)
    {
        return new Coordinate(p1.X*num, p1.Y*num);
    }

    public override string ToString()
    {
        return $"X_{X}, Y_{Y}";
    }
}