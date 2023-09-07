using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MazeBuilder : MonoBehaviour
{
    private MazeCell[,] maze;
    private Stack<Direction> returnStack = new Stack<Direction>();

    [SerializeField] private int rows = 5;
    [SerializeField] private int cols = 5;

    [SerializeField] private int seed = 9442064;
    [SerializeField] private bool isRandom = false;

    [SerializeField] private Vector2 startPosition = new Vector2(0, 0);
    [SerializeField] private Vector2 goalPosition = new Vector2(4, 4);
    [SerializeField] private bool customGoalPosition = false;
    [SerializeField] private bool randomGoalPosition = false;

    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject goal;
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject rabbit;
    [SerializeField] private GameObject mazeObj;
    [SerializeField] private Camera cam;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI mazeInfo;

    void Start(){
        generateMaze();
    }

    public void clearMaze(){
        for(int i = 0; i < mazeObj.transform.childCount; i++){
            GameObject.Destroy(mazeObj.transform.GetChild(i).gameObject);
        }
    }

    public void generateMaze(){
        clearMaze();
        if(isRandom){
            seed = UnityEngine.Random.Range(0, int.MaxValue);
        }
        UnityEngine.Random.InitState(seed);

        maze = new MazeCell[rows, cols];
        
        for(int i = 0; i < rows; i++){
            for(int j = 0; j < cols; j++){
                maze[i,j] = new MazeCell();
            }
        }
        
        buildMaze((int)startPosition.x, (int)startPosition.y);
        
        mazeInfo.text = String.Format("Semente: {0}\nTamanho: {1}x{2}\nInicio: ({3}, {4})\nChegada: ({6}, {5})", seed, rows, cols, (int)startPosition.x+1, (int)startPosition.y+1, (int)goalPosition.x+1, (int)goalPosition.y+1);
        maze[(int)startPosition.x, (int)startPosition.y].setStart();
        if(randomGoalPosition){
            goalPosition = new Vector2(UnityEngine.Random.Range(0, rows-1), UnityEngine.Random.Range(0, cols-1));
        }
        maze[(int)goalPosition.x, (int)goalPosition.y].setGoal();

        for(int i = 0; i < rows; i++){
            for(int j = 0; j < cols; j++){
                float x = j * 10;
                float z = i * 10;

                GameObject tmp = Instantiate(floor, new Vector3(x,0,z) + floor.transform.position, Quaternion.Euler(0, 0, 0), mazeObj.transform) as GameObject;

                if(maze[i, j].isGoal() > 0){
                    tmp.GetComponent<Renderer>().material.color = new Vector4(90/255f, 1, 0, 1);
                    Instantiate(goal, new Vector3(x,0,z), Quaternion.Euler(0, 0, 0), mazeObj.transform);
                }else if(maze[i, j].isStart() > 0){
                    GameObject rabbitObj = Instantiate(rabbit, new Vector3(x, 1.25f, z), Quaternion.Euler(0, 0, 0), mazeObj.transform) as GameObject;
                    MazeSolver rabbitControl = rabbitObj.GetComponent<MazeSolver>();
                    rabbitControl.currPoint = new Vector2(j, i);
                    rabbitControl.rows = rows;
                    rabbitControl.cols = cols;
                    rabbitControl.timerText = timerText;

                    tmp.GetComponent<Renderer>().material.color = new Vector4(1, 120/255f, 0, 1);
                }

                if(maze[i, j].wall(Direction.Left) > 0){
                    tmp = Instantiate(wall, new Vector3(x-5f, 0, z) + wall.transform.position, Quaternion.Euler(0, 90, 0), mazeObj.transform) as GameObject;
                }
                if(maze[i, j].wall(Direction.Up) > 0){
                    tmp = Instantiate(wall, new Vector3(x, 0, z-5f) + wall.transform.position, Quaternion.Euler(0, 0, 0), mazeObj.transform) as GameObject;
                }
                if(maze[i, j].wall(Direction.Right) > 0){
                    tmp = Instantiate(wall, new Vector3(x+5f, 0, z) + wall.transform.position, Quaternion.Euler(0, 90, 0), mazeObj.transform) as GameObject;
                }
                if(maze[i, j].wall(Direction.Down) > 0){
                    tmp = Instantiate(wall, new Vector3(x, 0, z+5f) + wall.transform.position, Quaternion.Euler(0, 0, 0), mazeObj.transform) as GameObject;
                }
            }
        }

        float y = 5;
        if(rows > cols){
            y = ((rows*10f) * .5f - .5f) + cols + rows;
        }else{
            y = ((cols*10f) * .5f - .5f) + cols + rows;
        }
        cam.transform.position = new Vector3(cols*5-5, y*2, rows*5-5);
    }

    void buildMaze(int i = 0, int j = 0){
        if(maze[i, j].isVisited() == 0){
            if(!customGoalPosition && !randomGoalPosition){
                goalPosition = new Vector2(i, j);
            }
        }
        maze[i,j].setVisited();
        List<Direction> neighbors = getUnvisitedNeighbors(i, j);
        Direction dir = Direction.None;
        if(neighbors.Count == 0){
            if(returnStack.Count > 0){
                dir = returnStack.Pop();
                // maze[i,j].disableWall(returnDir);
                // if(returnDir == Direction.Left){
                //     buildMaze(i, j-1);
                // }else if(returnDir == Direction.Up){
                //     buildMaze(i-1, j);
                // }else if(returnDir == Direction.Right){
                //     buildMaze(i, j+1);
                // }else if(returnDir == Direction.Down){
                //     buildMaze(i+1, j);
                // }
            }
        }else{
            dir = neighbors[UnityEngine.Random.Range(0, neighbors.Count)];
            // maze[i,j].disableWall(dir);
            returnStack.Push(invertDirection(dir));
            // if(dir == Direction.Left){
            //     buildMaze(i, j-1);
            // }else if(dir == Direction.Up){
            //     buildMaze(i-1, j);
            // }else if(dir == Direction.Right){
            //     buildMaze(i, j+1);
            // }else if(dir == Direction.Down){
            //     buildMaze(i+1, j);
            // }
        }
        if(!onBorder(i, j) && UnityEngine.Random.Range(0, 3) >= 1){
            maze[i, j].disableWall((Direction)(1 << UnityEngine.Random.Range(0, 4)));
        }
        maze[i,j].disableWall(dir);
        if(dir == Direction.Left){
            buildMaze(i, j-1);
        }else if(dir == Direction.Up){
            buildMaze(i-1, j);
        }else if(dir == Direction.Right){
            buildMaze(i, j+1);
        }else if(dir == Direction.Down){
            buildMaze(i+1, j);
        }
    }

    Direction invertDirection(Direction dir){
        if(dir == Direction.Left){
            return Direction.Right;
        }
        if(dir == Direction.Up){
            return Direction.Down;
        }
        if(dir == Direction.Right){
            return Direction.Left;
        }
        if(dir == Direction.Down){
            return Direction.Up;
        }
        return Direction.None;
    }

    List<Direction> getUnvisitedNeighbors(int i, int j){
        List<Direction> cells = new List<Direction>();
        if(i > 0 && maze[i-1, j].isVisited() == 0){
            cells.Add(Direction.Up);
        }
        if(i < rows-1 && maze[i+1, j].isVisited() == 0){
            cells.Add(Direction.Down);
        }
        if(j > 0 && maze[i, j-1].isVisited() == 0){
            cells.Add(Direction.Left);
        }
        if(j < cols-1 && maze[i, j+1].isVisited() == 0){
            cells.Add(Direction.Right);
        }
        return cells;
    }

    bool onBorder(int i, int j){
        return (i == 0 || i == rows-1 || j == 0 || j == cols-1);
    }
}
