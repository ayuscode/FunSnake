﻿[<FunScript.JS>]
module FunSnake
 
open FunScript
open FunScript.TypeScript
 
// ------------------------------------------------------------------
// Initialization
type Direction = Left | Right | Up | Down
let sizeLink = 10.
let canvasSize = (300., 300.)
let snake = [(10.,sizeLink,sizeLink,sizeLink);(sizeLink,sizeLink,sizeLink,sizeLink);(0.,sizeLink,sizeLink,sizeLink);]
let wallTop   = ([0.0..sizeLink..(fst canvasSize)] |> List.map (fun x-> (x,0.,sizeLink,sizeLink)))
let wallDown  = ([0.0..sizeLink..(fst canvasSize)] |> List.map (fun x-> (x,(snd canvasSize),sizeLink,sizeLink)))
let wallLeft  = ([0.0..sizeLink..(snd canvasSize)] |> List.map (fun x-> (0.,x,sizeLink,sizeLink)))
let wallRight = ([0.0..sizeLink..(snd canvasSize)] |> List.map (fun x-> ((fst canvasSize),x,sizeLink,sizeLink)))
let wall = wallLeft @ wallTop @ wallRight @ wallDown
let mutable direction = Right   // Control snake direction
let mutable moveDone = true     // Avoid direction changes until move has done
 
 
// ------------------------------------------------------------------
// Utils functions
 
// JQuery shortcut operator selector
let jQuery(selector : string) = Globals.Dollar.Invoke selector
let (?) jq name = jq("#" + name)
 
 
// Check if element exist in a generic list
let Contains (e:'T) (el:'T list) = el |> List.exists (fun x-> x = e)
 
// Get random number with max value
let getRandomAbsolute max absolute = (Globals.Math.floor( (Globals.Math.random() * max) / absolute)) * absolute
 
 
// ------------------------------------------------------------------
// Module program
 
// Move the snake to next position, if snake eat some food increase snake size in one link
let move xMove yMove snake food = match snake with
                                  | (x,y,h,w)::_ -> 
                                          let newHead = (x + xMove, y + yMove , h, w)
                                          if (newHead = food) then newHead :: snake
                                                              else newHead :: (snake |> List.rev |> Seq.skip 1 |> Seq.toList |> List.rev)
                                  | _ -> snake
 
// Move direction shortcuts
let moveRight snake food = move sizeLink 0. snake food
let moveLeft  snake food = move -sizeLink 0. snake food
let moveUp    snake food = move 0. -sizeLink snake   food
let moveDown  snake food = move 0. sizeLink snake  food
 
// Generate a new random food place (avoid wall & snake position)
let rec newFood snake () = 
                    let randomFood = ( (getRandomAbsolute ((fst canvasSize) - sizeLink * 2.) sizeLink) + sizeLink, (getRandomAbsolute ((snd canvasSize) - sizeLink * 2.) sizeLink) + sizeLink, sizeLink, sizeLink)
                    if snake |> Contains randomFood then newFood snake ()
                                                    else randomFood
 
// Detect snake collision (against wall or itself)
let hasCollision (snake:(float*float*float*float) List) = wall |> Contains snake.Head || snake.Tail |> Contains snake.Head
 
// Draw snake and food in the canvas
let draw (snake:(float*float*float*float) List, food:float*float*float*float, hasCollision: bool) =
    let canvas = jQuery?canvas.[0] :?> HTMLCanvasElement
    let ctx = canvas.getContext_2d()
    ctx.clearRect(sizeLink, sizeLink, fst canvasSize - (sizeLink), snd canvasSize - (sizeLink)) // Avoid reset the wall
 
    // Draw snake
    snake |> List.iter (fun x-> 
                                ctx.fillStyle <- if hasCollision then "rgb(255,0,0)" else "rgb(0,0,255)"
                                match x with
                                | x,y,w,h -> ctx.fillRect(x, y, w - 1., h - 1.)
                       ) |> ignore
 
    // Draw canvas
    ctx.fillStyle <- "rgb(0,255,0)"
    ctx.fillRect(food) |> ignore
 
// Draw the walls
let drawWall (wall:(float*float*float*float) List) =
    let canvas = jQuery?canvas.[0] :?> HTMLCanvasElement
    let ctx = canvas.getContext_2d()
    ctx.clearRect(0.0, 0.0, canvas.width, canvas.height)
 
    wall |> List.iter (fun x-> 
                                ctx.fillStyle <- "rgb(0,0,0)"
                                match x with
                                | x,y,w,h -> ctx.fillRect(x, y, w, h)
                        ) |> ignore
 
// ------------------------------------------------------------------
// Recursive update function that process the game
let rec update snake food () =
    // Snake position based on cursor direction input
    let snake = match direction with
                | Right -> moveRight snake food
                | Left -> moveLeft snake   food
                | Up -> moveUp snake       food
                | Down -> moveDown snake   food
 
    // If snake ate some food generate new random food
    let food = if (snake.Head = food) then newFood snake ()
               else food               
 
    // Detect snake collision        
    let collision = hasCollision snake
 
    // Draw snake & food in canvas (collision is use for paint snake in red in case of collision)
    draw (snake, food, collision) 
 
    // Snake movement completed
    moveDone <- true
    
    // If collision, game over, otherwise, continue updating the game
    if collision then 0
                 else Globals.setTimeout(update snake food, 1000. / 10.) |> ignore
                      1
 
 
// ------------------------------------------------------------------
// Main function
let main() = 
    // Capture arrows keys to move the snake
    Globals.window.addEventListener_keydown(fun e -> 
                                                    if moveDone then
                                                            if e.key = "Left"  && (direction = Up || direction = Down) then direction <- Left
                                                            if e.key = "Up"    && (direction = Right || direction = Left) then direction <- Up
                                                            if e.key = "Right" && (direction = Up || direction = Down) then direction <- Right
                                                            if e.key = "Down"  && (direction = Right || direction = Left) then direction <- Down
                                                            moveDone <- false
                                                    :> obj
                                           )
    // Draw the walls only once
    drawWall wall
 
    // Start the game with basic snake and ramdom food
    update snake (newFood snake ()) () |> ignore