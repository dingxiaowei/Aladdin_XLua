using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MainGameCSScript : MonoBehaviour
{
	const int cRow = 16;
	const int cCol = 10;
	List<int>[] aTable = new List<int>[cRow];
	int lineCount = 0;
	public Texture[] BoxTexture = new Texture[4];
	private string[] boxColors = new string[] { "eluosi_lan", "eluosi_zi", "eluosi_lv", "eluosi_hong" };

	public int LineCount
	{
		get { return lineCount; }
		set
		{
			lineCount = value;
			this.onLineCountChange();
		}
	}
	int score = 0;

	public int Score
	{
		get { return score; }
		set
		{
			score = value;
			this.onScoreChange();
		}
	}
	int nextCellID = 0;
	int curCellID = 0;

	float curSpeed = 1;
	float fCurSpeed = 1;
	float fDeltaTime = 0;
	bool bPause = false;

	bool bStart = false;
	bool bGameOver = false;

	public bool BGameOver
	{
		get { return bGameOver; }
		set
		{
			bGameOver = value;
			this.onGameOverChange(bGameOver);
		}
	}

	System.Random rdm = new System.Random();

	const int boxTop = 16;
	const int boxLeft = 290;
	/// <summary>
	/// 彩色砖块的宽度
	/// </summary>
	const int boxSize = 38;

	int[][] nextCell = null;
	int[][] curCell = null;
	int curCellCol = 0;
	int curCellRow = 0;

	public GameObject BtnUp, BtnDown, BtnLeft, BtnRight, BtnIsPause;
	public UILabel TimeLabel, ScoreLabel, LineCountLabel;

	private Action onScoreChange = null;
	private Action onLineCountChange = null;
	private Action<bool> onGameOverChange = null;
	public GameObject EndPanel, BtnRestart;
	public UISprite[] BoxSprites = null;

	void Start()
	{
		BoxSprites = new UISprite[4];
		for (int i = 0; i < 4; i++)
		{
			BoxTexture[i] = Resources.Load<Texture>(boxColors[i]);
		}
		this.onScoreChange = this.ScoreChange;
		this.onLineCountChange = this.LineCountChange;
		this.onGameOverChange = this.GameOverChange;
		TimeLabel = transform.Find("LeftNode/CutDownBG/CountLabel").GetComponent<UILabel>();
		LineCountLabel = transform.Find("ScoreNode/LineCount").GetComponent<UILabel>();
		ScoreLabel = transform.Find("ScoreNode/Score").GetComponent<UILabel>();
		BtnLeft = transform.Find("InputNode/Left").gameObject;
		BtnRight = transform.Find("InputNode/Right").gameObject;
		BtnUp = transform.Find("InputNode/Up").gameObject;
		BtnDown = transform.Find("InputNode/Down").gameObject;
		BtnIsPause = transform.Find("LeftNode/RestartBtn").gameObject;
		EndPanel = transform.Find("EndPanel").gameObject;
		BtnRestart = transform.Find("EndPanel/ReplayBtn").gameObject;

		for (int i = 1; i <= 4; i++)
		{
			var obj = transform.Find(string.Format("BlcokTop/Blocks/block{0}", i)).gameObject;
			var sprite = obj.GetComponent<UISprite>();
			BoxSprites[i - 1] = sprite;
		}

		UIEventListener.Get(BtnRestart).onClick = go =>
		{
			EndPanel.gameObject.SetActive(false);
			Debug.Log("重新开始");
			DoStart();
		};

		UIEventListener.Get(BtnIsPause).onClick = go =>
		{
			Debug.Log("开始/暂停");
			bPause = !bPause;
		};

		UIEventListener.Get(BtnUp).onClick = go =>
		{
			Debug.Log("上翻");
			DoTransform();
		};

		UIEventListener.Get(BtnDown).onPress = (go, state) =>
		{
			Debug.Log("下按");
			//state  true:按下   false:松开
			if (state)
				DoSpeedUp();
			else
				DoSpeedRestore();
		};

		UIEventListener.Get(BtnLeft).onClick = go =>
		{
			Debug.Log("左移");
			DoLeft();
		};

		UIEventListener.Get(BtnRight).onClick = go =>
		{
			Debug.Log("右移");
			DoRight();
		};

		this.DoStart();
	}

	void ScoreChange()
	{
		ScoreLabel.text = "获得积分:" + this.Score.ToString();
	}

	void LineCountChange()
	{
		LineCountLabel.text = "消灭行数:" + this.LineCount.ToString();
	}

	void GameOverChange(bool state)
	{
		if (state)
			EndPanel.SetActive(true);
	}

	/// <summary>
	/// 初始化10*16的格子
	/// </summary>
	void DoInit()
	{
		this.LineCount = 0;
		this.Score = 0;
		curSpeed = 1;

		for (int i = 0; i < aTable.Length; i++)
		{
			aTable[i] = new List<int>();
			for (int j = 0; j < cCol; j++)
				aTable[i].Add(0);
		}
	}

	void OnGUI()
	{
		if (!bStart)
			return;

		if (BGameOver)
		{
			return;
		}

		if (bPause)
		{
			return;
		}

		for (int i = 0; i < curCell.Length; i++)
		{
			for (int j = 0; j < curCell[i].Length; j++)
			{
				if (curCell[i][j] == 0)
				{
					continue;
				}
				if (i + curCellRow < 0)
				{
					continue;
				}
				GUI.DrawTexture(new Rect(boxLeft + boxSize * j + curCellCol * boxSize, boxTop + boxSize * i + curCellRow * boxSize, boxSize, boxSize), BoxTexture[0]);
			}
		}


		//背景图
		for (int i = 0; i < aTable.Length; i++)
		{
			for (int j = 0; j < cCol; j++)
			{
				if (aTable[i][j] == 0)
				{
					continue;
				}
				GUI.DrawTexture(new Rect(boxLeft + boxSize * j, boxTop + boxSize * i, boxSize, boxSize), BoxTexture[aTable[i][j] - 1]);
			}
		}
	}

	string GetContent(int cellContent)
	{
		switch (cellContent)
		{
			case 0:
				return "";
			default:
				return cellContent.ToString();
		}
	}

	void DoStart()
	{
		DoInit();
		DoNextCell();

		bStart = true;
		BGameOver = false;
	}

	void DrawNextBox()
	{
		//绘制下一个图
		int boxIndex = 0;
		for (int i = 0; i < nextCell.Length; i++)
		{
			for (int j = 0; j < nextCell[i].Length; j++)
			{
				if (nextCell[i][j] == 0)
				{
					continue;
				}
				BoxSprites[boxIndex++].transform.localPosition = new Vector3(j * 35, i * (-35), 0);
			}
		}
	}

	/// <summary>
	/// 随机生成下面一个模型
	/// </summary>
	void DoNextCell()
	{
		if (nextCell == null)
		{
			nextCellID = rdm.Next(aCells.Length);
			nextCell = aCells[nextCellID];
		}
		curCellCol = cCol / 2 - 2;
		curCellRow = -4;
		curCell = nextCell;
		curCellID = nextCellID;
		nextCellID = rdm.Next(aCells.Length);
		nextCell = aCells[nextCellID];

		DrawNextBox();

		DetectIsFail();
	}

	/// <summary>
	/// 放置当前的模型
	/// </summary>
	private void DoSetCurCellDown()
	{
		for (int i = 0; i < curCell.Length; i++)
		{
			for (int j = 0; j < curCell[i].Length; j++)
			{
				if (curCell[i][j] == 0)
				{
					continue;
				}
				if (curCellRow + i < 0)
				{
					BGameOver = true;
					return;
				}
				aTable[curCellRow + i][curCellCol + j] = curCell[i][j];
			}
		}
		//速度重置
		DoSpeedRestore();
		//消除一行
		DoLine();
		//生成下一个模型
		DoNextCell();
	}

	void Update()
	{
		if (!bStart)
		{
			return;
		}

		if (BGameOver)
		{
			return;
		}

		if (bPause)
		{
			return;
		}

		//自动下滑
		fDeltaTime += Time.deltaTime;
		if (fDeltaTime > fCurSpeed)
		{
			fDeltaTime -= fCurSpeed;
			if (CanMoveTo(curCellCol, curCellRow + 1))
			{
				curCellRow++;
			}
			else
			{
				DoSetCurCellDown();
				return;
			}
		}

		if (BGameOver)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))  //左移
		{
			DoLeft();
		}
		if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) //右移
		{
			DoRight();
		}
		if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))  //上翻
		{
			DoTransform();
		}
		if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))  //按下快速下移
		{
			DoSpeedUp();
		}
		if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))  //按键取消 取消快速下移
		{
			DoSpeedRestore();
		}
	}

	/// <summary>
	/// 检测游戏是否结束
	/// </summary>
	private void DetectIsFail()
	{
		if (CanMoveTo(curCellCol, curCellRow))
		{
			return;
		}
		BGameOver = true;
	}

	/// <summary>
	/// 判断模型移动
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="cell"></param>
	/// <returns></returns>
	bool CanMoveTo(int x, int y, int[][] cell = null) // 3, -4
	{
		if (cell == null)
		{
			cell = curCell;
		}
		for (int i = 0; i < cell.Length; i++)
		{
			for (int j = 0; j < cell[i].Length; j++)
			{
				if (cell[i][j] == 0)
				{
					continue;
				}
				if (x + j < 0 || x + j >= cCol)
				{
					return false;
				}
				if (y + i >= cRow) //到顶了
				{
					return false;
				}
				if (y + i < 0)
				{
					continue;
				}
				if (aTable[i + y][j + x] != 0)
				{
					return false;
				}
			}
		}
		return true;
	}

	private void DoSpeedRestore()
	{
		fCurSpeed = curSpeed;
	}

	//下移加速
	private void DoSpeedUp()
	{
		fCurSpeed = curSpeed / 8;
		fDeltaTime = fCurSpeed;
	}

	//右移
	private void DoRight()
	{
		if (CanMoveTo(curCellCol + 1, curCellRow))
		{
			curCellCol++;
		}
	}

	//左移
	private void DoLeft()
	{
		if (CanMoveTo(curCellCol - 1, curCellRow))
		{
			curCellCol--;
		}
	}

	//上翻
	private void DoTransform()
	{
		int transformedCellID = curCellID / 4 * 4 + (curCellID % 4 + 1) % 4;
		if (CanMoveTo(curCellCol, curCellRow, aCells[transformedCellID]))
		{
			curCellID = transformedCellID;
			curCell = aCells[transformedCellID];
		}
	}

	/// <summary>
	/// 消除一行
	/// </summary>
	private void DoLine()
	{
		List<int> fulledLines = new List<int>();
		for (int i = curCellRow; i < curCellRow + 4; i++)
		{
			if (i >= cRow)
			{
				continue;
			}
			if (i < 0)
			{
				BGameOver = true;
				return;
			}
			for (int j = 0; j < cCol; j++)
			{
				if (aTable[i][j] == 0)
				{
					break;
				}
				if (j == cCol - 1)
				{
					fulledLines.Add(i);
				}
			}
		}

		if (fulledLines.Count == 0)
		{
			return;
		}

		for (int i = 0; i < fulledLines.Count; i++)
		{
			for (int j = 0; j < cCol; j++)
			{
				aTable[fulledLines[i]][j] = 0;
			}
		}

		int ilastEmptyRow = fulledLines[fulledLines.Count - 1];
		int ilastNotEmptyRow = ilastEmptyRow - 1;
		while (true)
		{
			if (ilastNotEmptyRow < 0)
			{
				break;
			}

			if (!IsRowEmpty(ilastNotEmptyRow))
			{
				for (int j = 0; j < cCol; j++)
				{
					aTable[ilastEmptyRow][j] = aTable[ilastNotEmptyRow][j];
					aTable[ilastNotEmptyRow][j] = 0;
				}
				ilastEmptyRow--;
			}
			ilastNotEmptyRow--;
		}

		for (int i = 0; i < fulledLines.Count; i++)
		{
			for (int j = 0; j < cCol; j++)
			{
				aTable[i][j] = 0;
			}
		}
		//消除的行数
		LineCount += fulledLines.Count;
		//分数加成
		Score += fulledLines.Count * fulledLines.Count;

		curSpeed *= 0.99f;
	}

	bool IsRowEmpty(int irow)
	{
		for (int j = 0; j < cCol; j++)
		{
			if (aTable[irow][j] != 0)
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// 各种形态的砖块  28个
	/// </summary>
	private int[][][] aCells = new int[][][]
        {
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,1,1,0},
                        new int[]{0,1,1,0},
                        new int[]{0,0,0,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,1,1,0},
                        new int[]{0,1,1,0},
                        new int[]{0,0,0,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,1,1,0},
                        new int[]{0,1,1,0},
                        new int[]{0,0,0,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,1,1,0},
                        new int[]{0,1,1,0},
                        new int[]{0,0,0,0},
                },
 
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,0,1,1},
                        new int[]{0,1,1,0},
                        new int[]{0,0,0,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,1,0,0},
                        new int[]{0,1,1,0},
                        new int[]{0,0,1,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,0,1,1},
                        new int[]{0,1,1,0},
                        new int[]{0,0,0,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,1,0,0},
                        new int[]{0,1,1,0},
                        new int[]{0,0,1,0},
                },
 
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{1,1,0,0},
                        new int[]{0,1,1,0},
                        new int[]{0,0,0,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,0,1,0},
                        new int[]{0,1,1,0},
                        new int[]{0,1,0,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{1,1,0,0},
                        new int[]{0,1,1,0},
                        new int[]{0,0,0,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,0,1,0},
                        new int[]{0,1,1,0},
                        new int[]{0,1,0,0},
                },
 
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{1,1,1,1},
                        new int[]{0,0,0,0},
                        new int[]{0,0,0,0},
                },
                new int[][]
                {
                        new int[]{0,0,1,0},
                        new int[]{0,0,1,0},
                        new int[]{0,0,1,0},
                        new int[]{0,0,1,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{1,1,1,1},
                        new int[]{0,0,0,0},
                        new int[]{0,0,0,0},
                },
                new int[][]
                {
                        new int[]{0,0,1,0},
                        new int[]{0,0,1,0},
                        new int[]{0,0,1,0},
                        new int[]{0,0,1,0},
                },
 
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,1,0,0},
                        new int[]{1,1,1,0},
                        new int[]{0,0,0,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,1,0,0},
                        new int[]{0,1,1,0},
                        new int[]{0,1,0,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,0,0,0},
                        new int[]{1,1,1,0},
                        new int[]{0,1,0,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,1,0,0},
                        new int[]{1,1,0,0},
                        new int[]{0,1,0,0},
                },
 
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,1,1,0},
                        new int[]{0,0,1,0},
                        new int[]{0,0,1,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,0,1,0},
                        new int[]{1,1,1,0},
                        new int[]{0,0,0,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,1,0,0},
                        new int[]{0,1,0,0},
                        new int[]{0,1,1,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{1,1,1,0},
                        new int[]{1,0,0,0},
                        new int[]{0,0,0,0},
                },
 
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,1,1,0},
                        new int[]{0,1,0,0},
                        new int[]{0,1,0,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{1,1,1,0},
                        new int[]{0,0,1,0},
                        new int[]{0,0,0,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{0,0,1,0},
                        new int[]{0,0,1,0},
                        new int[]{0,1,1,0},
                },
                new int[][]
                {
                        new int[]{0,0,0,0},
                        new int[]{1,0,0,0},
                        new int[]{1,1,1,0},
                        new int[]{0,0,0,0},
                },
        };
}
