# NNDuong_UnityTest

# Scripts that I have worked on:
   
1. New scripts added:
+ BottomTrayControl
+ EventSO
+ UIPanelWin
  
2. Modified scripts:
+ Board
+ BoardController
+ Cell
+ Constant
+ GameManager
+ Item
+ LevelCondition
+ LevelMove
+ LevelTime
+ UIMainManager
+ UIPanelMain

# Explaination of task solving process:
   
1. Task 1: Re-skin
  Replace sprite in SpriteRenderer of itemNormal prefabs with fish sprites in Textures folder.
  
2. Task 2: Change the gameplay
  First I remove some features of the old gameplay such as Show Hint, Swap Item, etc...
  I create EventSO script for easy creation of event channel, make game object communicate with ease when some game event happpened.
+ Move items to bottom cells and Bottom area contains 5 cells:
  I created a new prefab, attached Grid Component and a script BottomTrayControl to it. The BottomTrayControl use Grid Component to initialize the "tray" containing 5 cells, storing item moving from initial boards.
+ If there are 3 indentical items, they will be cleared:
  The BottomTrayControl has a Dictionary to store NormalItem type when an item is added to the tray. If the quantity of a NormalItem type reach 3, they will be removed.
+ Clear the board to win:
  The BottomTrayControl has an integer of destroyed items. If it equal to the total items of the initial board, the player win.
+ Lose if bottom cells are filled:
  The BottomTrayControl has an integer represent number of items in the tray. If it equal to 5 then the player lose.
+ Identical items on initial board divisible by 3 and Task 3 - Ensure initial board contains all types of fish:
  I modified the Fill() function of the Board script. When Constructed, the board will first Instantiate all the NormalItem type, 3 instance of each type. Then it randomly pick one NormalItem type to add 3 instance of it to the board. This new function can be modified for differrent size of the board.
+ Show winning screen and Show losing screen
  I added a new UIPanelWin script. I modified LevelCondition script, UIMainManager script, GameManager script to correctly show the corresponding screen when player win or lose.
+ Autoplay button, each action has 0.5s delay.
  I modified UIPanelGame to add new button. I modified GameManager and BottomTrayControl to make the game enter Autoplay mode. To implement autoplay function, first I added new Dictionary to Board script, storing the Cell instance of each NormalItem type in the initial board. Then the game will traverse through each NormalItem type in this Dictionary, moving all item of that type to the tray. When there are no more item, it move on to the next NormalItem type. This process repeat until all the item have moved to the tray and destroyed, result in win.
+ Autolose button, eah action has 0.5s delay
  Same implementation as Autoplay button. The logic is that, the game only take one instance of NormalItem type in the Dictionary to the tray, then it moved on to the next NormalItem type. This will fill the tray with 5 different items, result in lose.

3. Task 3: Improve the gameplay
+ Add animation:
  I use the Move and Explode animation function which is already implemented in Cell and Item scripts.
+ Add Time Attack Mode:
  I modified GameManager, UIPanelMain, BottomTrayControl scripts to let the game enter Time Attack Mode state.
+ Player don't lose when bottom cells are filled:
  I added a bool check to BottomTrayControl script, so the player won't lose when the tray are full in Time Attack Mode.
+ Item can return to initial position on the board:
  I modified Item script, added a lastPosition Cell to it, which store the cell when the item is Instantiated in the board. If the item is clicked while on the tray, it will return to this lastPosition Cell.
+ Player Lose if fail to clear within 1 minute:
  I modified LevelTime script, so player will result in lose when (m_time <= -1f).



  
