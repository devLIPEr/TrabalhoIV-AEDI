using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell
{
    //8 bit for cell info
    //isStart    64
    //isGoal     32
    //isVisited  16
    //wallLeft    8
    //wallUp      4
    //wallRight   2
    //wallDown    1
    public sbyte info = 15; //All walls

    public sbyte isVisited(){
        return (sbyte)((info & 16) >> 4);
    }

    public void setVisited(){
        if(isVisited() == 0){
            info += 16;
        }
    }

    public sbyte isGoal(){
        return (sbyte)((info & 32) >> 5);
    }

    public void setGoal(){
        if(isGoal() == 0){
            info += 32;
        }
    }

    public sbyte isStart(){
        return (sbyte)((info & 64) >> 6);
    }

    public void setStart(){
        if(isStart() == 0){
            info += 64;
        }
    }

    public sbyte wall(Direction dir){
        return (sbyte)(info & (short)dir);
    }

    public void disableWall(Direction dir){
        if(wall(dir) > 0){
            info -= (sbyte)dir;
        }
    }
}
