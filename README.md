# Game Devlog

[Inventory Manager](#inventory-manager)

[Game Time Manager](#gametime-manager)

[Grid System](#grid-system)

[Scene Manager](#scene-manager)

[ISaveable](#isaveable)

[Crops](#crops)

[Player State Machine](#player-state-machine)

# Inventory Manager

- **Is-A:	 SingletonMonobehavior**
- **Has-A: ISaveable**

The inventory manger is inherited from singleton behavior just like all the other game mangers in our game. Since I'm the only programmer so it would be convenient for me to easily get access to all the other stuffs in the game. I still try to follow the rule of object oriented programming principle and keep things private as they should.

The inventory manager here contains serval functions like adding items, removing items, swapping items etc.

It also has a ISaveble interface for saving&loading inventory list before scene transition.

**Misc**

For now it only contains two inventory lists, one for player's main inventory and the other one is for the shop.

It should be extended in the near future to support multiple inventory lists so player can store their items in the treasure box they craft.

# GameTime Manager

- Is-A:	 SingletonMonobehavior
- Has-A: ISaveable

The time manager in the game is in charge of sending out signals to all the game time related stuffs, for example updating game clock UI, checking if a crop is mature or not, or to see if a quest is outdated etc.

**Misc**

Here in setting, 10 minutes in game time roughly equal to 5 seconds in real life. Since the game time manager works pretty closed to event handler, probably not a good idea to send out events every 5 seconds. Right now it has no impact to the performance so I'm guessing it is fine.

But should try to avoid using event handler in the future even though it's convenient, it's would be super difficult to trace who got notified afterwards, or when the project grows bigger and bigger. A self-build event system might be a better choice but still not a promising way to do this.

# Grid System

- Is-A: basic class

In a 2D rpg game, we see everything and store everything in the grid. Thus it's very important that we make our own customized grid system. As in the grid system we define some basic details of the grid, for example grid's coordinations, isDiggable or isWalkable.

After we have set up the basic grid class, we store the informations of each grid of each scene into a List which is a scriptable object by using customized tilemap drawer. So everytime we run the game, we already got all the grids' informaiton for later use. Here the grid manager also has a ISaveable interface so we can store their details into binary file just like any other managers. We do not use scriptable object for saving data only for initialization just for consistency.

# Scene Manager

Before getting into the save system in our game, first let's take a look at the scene manager. In most of the game when player gets into a new scene, we need to preserve the old scene's data and load new scene's data. For example the items player drops, the crops player plants etc should be recorded for future loading.

We use the fading effect happens when switching scenes as a sign when to save/load the data. When the scenes starts to fade out, we now store the current scene's data and start fading out. When the scene is fading out to full black, we now load the next scene. And after we get into new scene(now the scene is still full black since we haven't start fading in), we restore the new scene's data and then start fading in effect. In this way we can have a smooth scene transition.

#### Misc

Right now the duration time of fading effect is fixed, it will be better if it can real fit the time you need to transit to the next scene.

# ISaveable

Since now we know when to trigger the save/load functions, we go into details of how they're implemented.

All game managers that need to be saved have a interface called ISaveable. Whenever a script is considered to have ISaveable interface, it will be automatically added into a List. Then when we need to store/restore the scene data, we just need to loop through the List and for each element, call the corresponding store/restore function that defined in ISaveable interface.

How we gonna store data? Actually there are different sorts of data types that we need to store. So here in we create a new class called SceneSave that contains hash maps of different data types. For example, we have `Dictionary<string, Vector3Serializable> vector3Dictionary;`to store the position of player or any other varibles that is Vector3. And`Dictionary<string, GridDetails> gridDetailsDictionary;` to store the information of grids of the scene it belongs to. In this way, when calling the store function, we just need to save one SceneSave class that contains all the data types we need to save. After we got the SceneSave data, we bind it with its corresponding scene name and put them into a dictionary.  Now we successfully get the data we need for each scene. And whenever we need to store the SceneSave that we already store (that is, visit scene again like SceneA->B->A), we delete the old one and push the newly created one in.

How we gonna restore data? As mentioned above, we store SceneSave along with its scene name in a dictionary, so when loading a new scene, we try to fetch its SceneSave and extract out its data for our needs. Simple as it is.

So above is how we store/restore data when switching scenes. While player should feel free to save the data whenever they want, that is when they press the save game button. The logic here remains the same.

# Crops

The crops we plant in our game are just like all the other self defined objects, they have their own coordination and details. And we use scriptable objects as a database to pre-define and initialize crops.

As it should be, some of the crops' information like how many days since it gets watered, how many days has it grown, things like this is stored in the grid it is planted. Thus intuitively, we put our display crops/ clear crops functions in the grid manager. The functions here are quite simple and easy to understand so we don't get into details of it.

One thing to mention that is, when player move into the next day, the crops should update its information (that is stored in corresponding grid). We do this by trigger a event from Time manager. 

#### Misc

 (*While it's actually not a good practice to have crop related functions inside grid manager which results in the grid manager grows bigger and bigger and that's something we should change/avoid in the future. We probably should make a new crop manager. Try to keep scripts as clean as possible only do what's supposed to do instead of stepping feet in the other fields*)

# Player State Machine

Here in game player might have several animation states, hence we write our own state machine and combine it with the animation check conditions. Each state also works close to grid system, for example when the player is about to chop a tree, first it would trigger the animation when entering the state, then we modify the values to the grid player just chopped, and after animation is done, we decide if that tree should be cut down or not. Essentially all the states are quite similar in the aspect the logic.

#### Misc

It's better to create own state machine instead of binding functions to unity-animations so it would be easier to debu

# NPC

