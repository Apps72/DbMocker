using System;

namespace Apps72.Dev.Data.DbMocker
{
    public static class ArrayExtensions
    {
        /// <summary/>
        public static T[][] ToJaggedArray<T>(this T[,] twoDimensionalArray)
        {
            if (twoDimensionalArray == null)
                return null;

            int rowsFirstIndex = twoDimensionalArray.GetLowerBound(0);
            int rowsLastIndex = twoDimensionalArray.GetUpperBound(0);
            int numberOfRows = rowsLastIndex + 1;

            int columnsFirstIndex = twoDimensionalArray.GetLowerBound(1);
            int columnsLastIndex = twoDimensionalArray.GetUpperBound(1);
            int numberOfColumns = columnsLastIndex + 1;

            T[][] jaggedArray = new T[numberOfRows][];
            for (int i = rowsFirstIndex; i <= rowsLastIndex; i++)
            {
                jaggedArray[i] = new T[numberOfColumns];

                for (int j = columnsFirstIndex; j <= columnsLastIndex; j++)
                {
                    jaggedArray[i][j] = twoDimensionalArray[i, j];
                }
            }
            return jaggedArray;
        }

        /// <summary/>
        public static T[,] ToTwoDimensionalArray<T>(this T[][] jaggedArray)
        {
            if (jaggedArray == null)
                return null;

            int firstDimension = jaggedArray.Length;
            int secondDimension = 0;

            foreach (var row in jaggedArray)
            {
                if (row.Length > secondDimension)
                    secondDimension = row.Length;
            }

            T[,] twoDimensionalArray = new T[firstDimension, secondDimension];

            for (int i = 0; i < firstDimension; i++)
            {
                for (int j = 0; j < secondDimension; j++)
                {
                    twoDimensionalArray[i, j] = jaggedArray[i][j];
                }
            }

            return twoDimensionalArray;

        }

        /// <summary/>
        public static int ColumnsCount(this object[,] twoDimensionalArray)
        {
            return twoDimensionalArray.GetLength(1);
        }

        /// <summary/>
        public static int RowsCount(this object[,] twoDimensionalArray)
        {
            return twoDimensionalArray.GetLength(0);
        }
    }
}
