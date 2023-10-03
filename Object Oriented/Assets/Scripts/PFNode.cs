using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFNode
{
    public int x;
        public int y;
        public int hCost;
        public int gCost;

        public bool walkable;
        public PFNode Parent;


        public PFNode(bool walkable, int x, int y)
        {
            this.walkable = walkable;
            this.x = x;
            this.y = y;
            gCost = int.MaxValue;
        }

        public int FCost
        {
            get
            {
                return gCost + hCost;
            }
        }
}
