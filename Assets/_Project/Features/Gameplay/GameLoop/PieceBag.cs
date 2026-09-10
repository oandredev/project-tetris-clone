using System.Collections.Generic;
using UnityEngine;

namespace Features.Gameplay.GameLoop
{
    public class PieceBag
    {
        private readonly Piece.Piece[] allPieces;
        private List<Piece.Piece> currentBag = new List<Piece.Piece>();

        public PieceBag(Piece.Piece[] pieces)
        {
            allPieces = pieces;
        }

        public Piece.Piece Next()
        {
            EnsureBagFilled();
            Piece.Piece next = currentBag[0];
            currentBag.RemoveAt(0);
            return next;
        }

        public Piece.Piece Peek()
        {
            EnsureBagFilled();
            return currentBag[0];
        }

        private void EnsureBagFilled()
        {
            if (currentBag.Count > 0) return;

            currentBag = new List<Piece.Piece>(allPieces);
            Shuffle(currentBag);
        }

        private void Shuffle(List<Piece.Piece> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}