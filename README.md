The video is a simple project showing a compute shader's power to get a considerable performance boost.
We have instantiated 1500 cubes with custom collider data in the scene and, we are checking the collision b/w them via sphere to sphere collision checks and coloring the cubes magenta if the 2 cubes collide.
This would result in having to do 2,250,000 collision checks per frame.
For a CPU this can be very resource-intensive. We can instead offload the task to GPU and take advantage of its parallel processing power by having the collision check done on a compute shader.
Video : https://www.youtube.com/watch?v=grJo25WCr7Y 
