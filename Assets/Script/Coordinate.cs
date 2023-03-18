using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public struct Coordinate : INetworkSerializable, IEquatable<Coordinate>
{
    public int X;
    public int Y;
    public static Coordinate none = new Coordinate(-1, -1);

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

    public static bool operator==(Coordinate p1, Coordinate p2)
    {
        return p1.X == p2.X && p1.Y == p2.Y;
    }

    public static bool operator!=(Coordinate p1, Coordinate p2)
    {
        return p1.X != p2.X || p1.Y != p2.Y;
    }

    public override string ToString()
    {
        return $"X_{X}, Y_{Y}";
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public bool Equals(Coordinate other)
    {
        return X == other.X && Y == other.Y;
    }
}