# Solution

## How to Run
You can test the game by opening the **Main Scene** and pressing **Play**.

If needed, you can also inspect or fine-tune some of the gameplay and generation values through the **GameConfig** Scriptable Object.

## Known Issue
While playing, you may occasionally encounter linked balls that cannot be resolved. If that happens, you can restart the game and continue.
Due to time constraints, I was not able to fully complete a system that guarantees all generated links are always solvable in every scenario.

---

## Explanations

---
## About Creating Balls
I began reading the generated pixel art in layers, from the outer parts toward the center. Whenever certain chunk sizes were reached, I generated `BigBallData` objects.
For example, once 50 pixels were read, I created `BigBallData` entries based on the color distribution within those 50 pixels. This process continued until all pixels had been processed. 
Any remaining data from the previous chunk was carried over into the next one, ensuring that `BigBallData` objects were generated for the entire pixel art.

There are some parts of the code inside the `BallController` script that I believe should be refactored into more appropriate places, but I did not have enough time to clean that up properly. (`BallController`)

---

For the link generation part, I used several filtering approaches and fine-tuned the system with configurable variables. However, due to limited time, some scenarios can still produce unsolvable links. (`BallLaneController`, line 104)

---

While creating the border of the pixel art area, I used **Dreamteck Splines**’ mesh generation system. This allowed me to create a border with both smooth corners and a scalable structure. The related values can be adjusted through **GameConfig**. 
However, due to time constraints, the camera and scene layout remained static, so in some cases elements may overflow outside the screen. (`PixelAreaController`, line 105)

---

## External Tools / Packages / Assistance Used

### DOTween Pro
DOTween Pro was used to improve the overall game feel, particularly for object movement, bounce/jump effects, and scaling animations.

### Hot Reload
Hot Reload was used to accelerate the prototyping process by allowing code changes to be applied instantly during development.

### Sirenix Odin Inspector
Sirenix Odin Inspector was used to create cleaner and more manageable Inspector layouts, improving data organization and editing workflow in Unity.

### TextMesh Pro
TextMesh Pro was used to create sharp and readable world-space text elements within the game scene.

### Dreamteck Splines
Dreamteck Splines was used to generate a scalable custom mesh for the pixel-art border lines.

### ChatGPT
ChatGPT was used as a guidance tool for improving the balls’ movement and bounce behavior.

In addition, previously written spline point generation code from older projects was reused and modified to create the border for the pixel area.
