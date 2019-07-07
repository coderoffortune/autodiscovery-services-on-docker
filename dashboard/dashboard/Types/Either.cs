using System;
namespace dashboard.Types
{
    public class Either<A, B>
    {
        public A left { get; }
        public B right { get; }

        public Either(A left)
        {
            this.left = left;
            this.right = default(B);
        }

        public Either(B right)
        {
            this.left = default(A);
            this.right = right;
        }

        public bool IsRight()
        {
            return right != null;
        }
    }

    public class Left<A> : Either<A, Nothing>
    {
        public Left(A left) : base(left) { }
    }

    public class Right<B> : Either<Nothing, B>
    {
        public Right(B right) : base(right) { } 
    }
}
