using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WallJumper 
{
    public static class Collision
    {
       public static bool RectangleCollision(Vector2 firstPosition
                    , float firstWidth 
                    , float firstHeight 
                    , Vector2 secondPosition
                    , float secondWidth
                    , float secondHeight)
               {
                   return firstPosition.X < (secondPosition.X + secondWidth)
                          && (firstPosition.X + firstWidth) > secondPosition.X 
                          && firstPosition.Y < (secondPosition.Y + secondHeight)
                          && (firstPosition.Y + firstHeight) > secondPosition.Y;
               }
    }
}