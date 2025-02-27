﻿using JetBrains.Annotations;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System;
using System.Threading;
using UnityEngine;

namespace Project.UI
{
    [UsedImplicitly]
    public sealed class OpponentInputPresenter
    {        
        private string _selectedSymbol;

        public async void SelectSymbol()
        {
            var symbols = Symbol.GetSymbols();
            int selection = await GetRandomSelection(0, symbols.Length -1);

            _selectedSymbol = selection >= 0 ? symbols[selection] : "";
        }

        public void Reset()
        {
            _selectedSymbol = "";
        }

        public string GetSelectedSymbol()
        {
            return _selectedSymbol;
        }

        public async UniTask<int> GetRandomSelection(int min, int max)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(3));

            try
            {
                var txt = (await UnityWebRequest.Get($"http://www.randomnumberapi.com/api/v1.0/random?min={min}&max={max}").SendWebRequest()).downloadHandler.text;

                var result = txt.Trim(new[] { '[', ']' });

                if (int.TryParse(result, out int selection))
                {
                    return selection;
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Opponent GetRandomSelection timed out");
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }

            // Fall back to random number generation if failed to get random number number
            return UnityEngine.Random.Range(min, max + 1);
        }
    }
}