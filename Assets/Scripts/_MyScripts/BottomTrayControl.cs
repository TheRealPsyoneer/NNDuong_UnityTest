using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEditor.Progress;
using static NormalItem;
using System.Reflection;

public class BottomTrayControl : MonoBehaviour
{
    public static BottomTrayControl Instance;

    Grid grid;
    public int trayCapacity;
    [SerializeField] EventSO itemClickMoveToTrayEvent;
    [SerializeField] EventSO levelLoseEvent;
    [SerializeField] EventSO levelWinEvent;
    [SerializeField] EventSO autoEvent;
    [SerializeField] EventSO endAutoEvent;

    List<Cell> cellList;
    public int currentQuantity;
    Dictionary<eNormalType, int> itemQuantity;

    [SerializeField] GameSettings gameSettings;
    int totalItems;
    int currentDestroyedItems;

    public bool isAutoplay;
    public bool isAutolose;

    private void Awake()
    {
        Instance = this;

        grid = GetComponent<Grid>();
        gameObject.SetActive(true);
        cellList = new List<Cell>();
        InitializeTray();

        totalItems = gameSettings.BoardSizeX * gameSettings.BoardSizeY;
        currentQuantity = 0;
        itemQuantity = new Dictionary<eNormalType, int>();

    }

    public void StartAutoplay()
    {
        int startNum = 0;
        StartCoroutine(ExecutingAutoplay(startNum));
    }

    IEnumerator ExecutingAutoplay(int startNum)
    {
        yield return new WaitForSeconds(0.5f);
        autoEvent.Broadcast();
        while (true)
        {
            eNormalType type = (eNormalType)startNum;
            if (BoardController.instance.Board.itemCells[type].Count == 0)
            {
                startNum++;
            }
            if (startNum >= BoardController.instance.Board.itemCells.Count)
            {
                endAutoEvent.Broadcast();
                break;
            }

            Cell cell = BoardController.instance.Board.itemCells[(eNormalType)startNum][0];
            BoardController.instance.Board.itemCells[(eNormalType)startNum].Remove(cell);
            MoveItemToTray(cell);

            yield return new WaitForSeconds(0.5f);
        }

    }

    public void StartAutolose()
    {
        StartCoroutine(ExecutingAutolose());
    }

    IEnumerator ExecutingAutolose()
    {
        yield return new WaitForSeconds(0.5f);
        autoEvent.Broadcast();
        int startNum = 0;
        while (true)
        {
            eNormalType type = (eNormalType)startNum;

            Cell cell = BoardController.instance.Board.itemCells[(eNormalType)startNum][0];
            BoardController.instance.Board.itemCells[(eNormalType)startNum].Remove(cell);
            MoveItemToTray(cell);
            startNum++;

            if (currentQuantity == trayCapacity)
            {
                endAutoEvent.Broadcast();
                break;
            }

            yield return new WaitForSeconds(0.5f);

            
        }

    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        itemClickMoveToTrayEvent.ThingHappenedCell += MoveItemToTray;
    }

    private void OnDestroy()
    {
        itemClickMoveToTrayEvent.ThingHappenedCell -= MoveItemToTray;
    }

    //private void OnDisable()
    //{
    //    gameStartEvent.ThingHappened -= OnGameStart;
    //}


    void InitializeTray()
    {
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);

        for (int i = 0; i < trayCapacity; i++)
        {
            GameObject go = GameObject.Instantiate(prefabBG);

            Cell cell = go.GetComponent<Cell>();
            cellList.Add(cell);

            go.transform.position = grid.CellToWorld(new Vector3Int(i, 0, 0));

            if (GameManager.Instance.LevelMode != GameManager.eLevelMode.TIMER)
            {
                cell.GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    void MoveItemToTray(Cell cell)
    {
        if (cellList.Contains(cell))
        {
            MoveItemBack(cell);
            return;
        }


        if (currentQuantity < trayCapacity)
        {
            Cell targetCell = cellList[currentQuantity];
            cell.Item.SetCell(targetCell);
            targetCell.Assign(cell.Item);
            cell.Free();

            targetCell.ApplyItemMoveToPosition();

            NormalItem normalItem = targetCell.Item as NormalItem;

            if (!itemQuantity.ContainsKey(normalItem.ItemType))
            {
                itemQuantity[normalItem.ItemType] = 0;
            }
            itemQuantity[normalItem.ItemType]++;
            currentQuantity++;

            if (itemQuantity[normalItem.ItemType] == 3)
            {
                for (int i = 0; i < currentQuantity; i++)
                {
                    NormalItem curItem = cellList[i].Item as NormalItem;
                    if (curItem.ItemType == normalItem.ItemType)
                    {
                        cellList[i].Item.ExplodeView();
                        cellList[i].Free();
                    }
                }

                itemQuantity.Remove(normalItem.ItemType);
                currentQuantity -= 3;
                currentDestroyedItems += 3;

                int index = 0;
                for (int i = 0; i < cellList.Count; i++)
                {
                    if (cellList[i].Item != null && i == index)
                    {
                        index++;
                        continue;
                    }
                    else if (cellList[i].Item == null) continue;
                    else
                    {
                        cellList[index].Assign(cellList[i].Item);
                        cellList[i].Free();
                        cellList[index].ApplyItemMoveToPosition();
                        index++;
                    }
                }
            }

            if (GameManager.Instance.LevelMode != GameManager.eLevelMode.TIMER && currentQuantity == trayCapacity)
            {
                levelLoseEvent.Broadcast();
                StartCoroutine(DestroyDelay());
            }
            if (currentDestroyedItems >= totalItems)
            {
                levelWinEvent.Broadcast();
                StartCoroutine(DestroyDelay());
            }
        }
    }

    public void SelfDestruct()
    {
        StartCoroutine(DestroyDelay());
    }

    IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < cellList.Count; i++)
        {
            Destroy(cellList[i].gameObject);
        }
        Destroy(gameObject);
    }

    void MoveItemBack(Cell cell)
    {
        Cell targetCell = cell.Item.lastPosition;
        cell.Item.SetCell(targetCell);
        targetCell.Assign(cell.Item);
        cell.Free();
        targetCell.ApplyItemMoveToPosition();

        int index = 0;
        for (int i = 0; i < cellList.Count; i++)
        {
            if (cellList[i].Item != null && i == index)
            {
                index++;
                continue;
            }
            else if (cellList[i].Item == null) continue;
            else
            {
                cellList[index].Assign(cellList[i].Item);
                cellList[i].Free();
                cellList[index].ApplyItemMoveToPosition();
                index++;
            }
        }

        currentQuantity--;
        NormalItem nitem = targetCell.Item as NormalItem;
        itemQuantity[nitem.ItemType]--;
    }
}
