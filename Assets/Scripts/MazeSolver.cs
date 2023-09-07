using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MazeSolver : MonoBehaviour
{
    public bool goal = false;
    public int rows, cols;
    public float waitTime = 0.1f, timer = 0;
    public Vector2 currPoint;
    private MazeCell[,] maze;
    Stack<Direction> dirs = new Stack<Direction>();
    public TextMeshProUGUI timerText;

    void Start(){
        dirs.Clear();
        maze = new MazeCell[rows,cols];
        for(int i = 0; i < rows; i++){
            for(int j = 0; j < cols; j++){
                maze[i,j] = new MazeCell();
            }
        }

        this.gameObject.transform.position = new Vector3((currPoint.y)*10, 1.25f, (currPoint.x)*10);

        StartCoroutine(FindExit());
    }

    void FixedUpdate() {
        if(!goal){
            timer += Time.deltaTime;
        }
        timerText.text = timer.ToString("F2") + "s";
    }

    IEnumerator FindExit(){
        yield return new WaitForSeconds(waitTime);
        maze[(int)currPoint.y, (int)currPoint.x].setVisited();

        //Check for walls
        RaycastHit hit;
        if(!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5f)){
            maze[(int)currPoint.y, (int)currPoint.x].disableWall(Direction.Down);
        }
        if(!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit, 5f)){
            maze[(int)currPoint.y, (int)currPoint.x].disableWall(Direction.Up);
        }
        if(!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 5f)){
            maze[(int)currPoint.y, (int)currPoint.x].disableWall(Direction.Left);
        }
        if(!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 5f)){
            maze[(int)currPoint.y, (int)currPoint.x].disableWall(Direction.Right);
        }

        //Solve maze
        List<Direction> neighbors = getUnvisitedNeighbors((int)currPoint.y, (int)currPoint.x);
        if(neighbors.Count > 0){
            Direction dir = neighbors[Random.Range(0, neighbors.Count)];
            dirs.Push(invertDirection(dir));
            move(dir);
        }else{
            if(dirs.Count > 0){
                Direction returnDir = dirs.Pop();
                move(returnDir);
            }
        }

        if(!goal){
            transform.position = new Vector3((currPoint.x)*10, 1.25f, (currPoint.y)*10);
            StartCoroutine(FindExit());
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

    void move(Direction dir){
        if(dir == Direction.Up){
            currPoint.y = (int)currPoint.y - 1;
            transform.GetChild(0).gameObject.transform.rotation = Quaternion.Euler(new Vector3(-90, 180, -180));
        }else if(dir == Direction.Down){
            currPoint.y = (int)currPoint.y + 1;
            transform.GetChild(0).gameObject.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, -180));
        }else if(dir == Direction.Left){
            currPoint.x = (int)currPoint.x - 1;
            transform.GetChild(0).gameObject.transform.rotation = Quaternion.Euler(new Vector3(-90, 270, -180));
        }else if(dir == Direction.Right){
            currPoint.x = (int)currPoint.x + 1;
            transform.GetChild(0).gameObject.transform.rotation = Quaternion.Euler(new Vector3(-90, 90, -180));
        }
    }

    List<Direction> getUnvisitedNeighbors(int i, int j){
        List<Direction> cells = new List<Direction>();
        if(i > 0 && maze[i-1, j].isVisited() == 0 && maze[i, j].wall(Direction.Up) == 0){
            cells.Add(Direction.Up);
        }
        if(i < rows-1 && maze[i+1, j].isVisited() == 0 && maze[i, j].wall(Direction.Down) == 0){
            cells.Add(Direction.Down);
        }
        if(j > 0 && maze[i, j-1].isVisited() == 0 && maze[i, j].wall(Direction.Left) == 0){
            cells.Add(Direction.Left);
        }
        if(j < cols-1 && maze[i, j+1].isVisited() == 0 && maze[i, j].wall(Direction.Right) == 0){
            cells.Add(Direction.Right);
        }
        return cells;
    }
}
