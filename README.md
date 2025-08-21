# Fishing Game

A simple 2D fishing game built using MonoGame and C#. Cast your rod, catch fish, earn points and track your fishing success across sessions with persistent save data.


## Gameplay

* Press Space to cast your line.
* Wait for a fish to bite.
* When a fish bites, press Space again within 2 seconds to catch it.


## Features

* Pressing Tab shows caught fish.
* Three different catches:
  * Flounder (8 points)
  * Perch (3 points)
  * Tin Can (1 point)

* Persistent save data
  * Automatically saves caught fish and total score to _catch_list.json_.
  * Loads your progress each time the game starts.
  * Pressing R resets progress



## Images

Waiting for the fish to bite

<img src="https://github.com/RiinaP/FishingGame/blob/main/Images/Screenshot1.png" alt="Screenshot of the initial state of the game">

The fish bites! You have two seconds to catch it before it gets away.


<img src="https://github.com/RiinaP/FishingGame/blob/main/Images/Screenshot2.png" alt="Screenshot of the moment when a fish bites">

The list of all the fish you've caught so far.


<img src="https://github.com/RiinaP/FishingGame/blob/main/Images/Screenshot3.png" alt="Screenshot of the results screen">

The basic structure of the game.

<img src="https://github.com/RiinaP/FishingGame/blob/main/Images/Flowchart.png" alt="Flowchart that outlines the structure of the game">



## Code

The main loop of the game consists of multiple timers. If fishing rod has been cast, it starts a timer to see when the fish will bite. Once it does, another timer checks if the fish is then caught within a certain time limit. Should the player succeed, the fish will be added to a list of catches and the game saves its state and sets up the timers and randomisers for the next round.
```
if (_fishingRodCast)
{
    _biteTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

    if (!_fishBites)
    {
        if (_biteTimer > _biteInterval)
        {
            _fishBites = true;
            _catchTimer = _catchTimerLimit;
        }
    }
    else
    {
        _catchTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (IsKeyPressed(Keys.Space))
        {
            _caughtFish = _nextFish;
            _fishPoints += _caughtFish.Points;

            string name = _caughtFish.Name;
            if (_catchList.ContainsKey(name))
            {
                _catchList[name]++;
            }
            else
            {
                _catchList[name] = 1;
            }

            SaveGame();
            ResetFish();
        }

        if (_catchTimer <= 0)
        {
            ResetFish();
            _fishEscaped = true;
        }
    }
}
```


## Development ideas

The game could be further improved by adding features such as:

* Weather cycle
* Day/night cycle
* More types of fish
* Lures and fishing rod upgrades
* Animations
* Different areas to fish in
* A fishing minigame, with the difficulty adjusted for each fish separately