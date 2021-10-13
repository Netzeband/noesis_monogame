using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NoesisLib = Noesis;


namespace UI.Noesis.Input
{
    public class NoesisKeyboardInputHandler : Input.INoesisKeyboardHandler
    {
        private readonly Dictionary<Keys, TimeSpan> _pressedKeyTime = new();
        private readonly TimeSpan _keyRepeatTime = TimeSpan.FromMilliseconds(200);
        private NoesisLib.View _view;
        
        public void Init(global::Noesis.View view)
        {
            _view = view;
        }

        public void UnInit()
        {
            _view = null;
        }

        public Keys[] ProcessKeys(Keys[] pressedKeys, GameTime gameTime)
        {
            var unconsumedKeys = new List<Keys>();

            foreach (var key in pressedKeys.Concat(_pressedKeyTime.Keys))
            {
                if (pressedKeys.Contains(key) && !_pressedKeyTime.ContainsKey(key))
                {
                    ProcessKeyDown(key, gameTime, ref unconsumedKeys);
                }
                else if (!pressedKeys.Contains(key) && _pressedKeyTime.ContainsKey(key))
                {
                    ProcessKeyUp(key);
                }
                else
                {
                    ProcessKeyRepeat(key, gameTime);
                }
            }
            
            return unconsumedKeys.ToArray();
        }
        
        private void ProcessKeyDown(Keys key, GameTime gameTime, ref List<Keys> unconsumedKeys)
        {
            var noesisKey = GetNoesisKey(key); 

            if (_view.KeyDown(noesisKey))
            {
                _pressedKeyTime[key] = gameTime.TotalGameTime;
            }
            else
            {
                unconsumedKeys.Add(key);
            }
        }
        
        
        private void ProcessKeyUp(Keys key)
        {
            var noesisKey = GetNoesisKey(key); 

            _view.KeyUp(noesisKey);
            _pressedKeyTime.Remove(key);
        }
        
        
        private void ProcessKeyRepeat(Keys key, GameTime gameTime)
        {
            if (gameTime.TotalGameTime - _pressedKeyTime[key] > _keyRepeatTime)
            {
                var noesisKey = GetNoesisKey(key);
                _view.KeyDown(noesisKey);
                _pressedKeyTime[key] += _keyRepeatTime;
            }
        }

        
        private static NoesisLib.Key GetNoesisKey(Keys key)
        {
            switch (key)
            {
                case Keys.Back: return NoesisLib.Key.Back;
                case Keys.Tab: return NoesisLib.Key.Tab;
                case Keys.Enter: return NoesisLib.Key.Enter;
                case Keys.Pause: return NoesisLib.Key.Pause;
                case Keys.CapsLock: return NoesisLib.Key.CapsLock;
                case Keys.Kana: return NoesisLib.Key.KanaMode;
                case Keys.Kanji: return NoesisLib.Key.KanjiMode;
                case Keys.Escape: return NoesisLib.Key.Escape;
                case Keys.ImeConvert: return NoesisLib.Key.ImeConvert;
                case Keys.ImeNoConvert: return NoesisLib.Key.ImeNonConvert;
                case Keys.Space: return NoesisLib.Key.Space;
                case Keys.PageUp: return NoesisLib.Key.PageUp;
                case Keys.PageDown: return NoesisLib.Key.PageDown;
                case Keys.End: return NoesisLib.Key.End;
                case Keys.Home: return NoesisLib.Key.Home;
                case Keys.Left: return NoesisLib.Key.Left;
                case Keys.Up: return NoesisLib.Key.Up;
                case Keys.Right: return NoesisLib.Key.Right;
                case Keys.Down: return NoesisLib.Key.Down;
                case Keys.Select: return NoesisLib.Key.Select;
                case Keys.Print: return NoesisLib.Key.Print;
                case Keys.Execute: return NoesisLib.Key.Execute;
                case Keys.PrintScreen: return NoesisLib.Key.PrintScreen;
                case Keys.Insert: return NoesisLib.Key.Insert;
                case Keys.Delete: return NoesisLib.Key.Delete;
                case Keys.Help: return NoesisLib.Key.Help;
                case Keys.D0: return NoesisLib.Key.D0;
                case Keys.D1: return NoesisLib.Key.D1;
                case Keys.D2: return NoesisLib.Key.D2;
                case Keys.D3: return NoesisLib.Key.D3;
                case Keys.D4: return NoesisLib.Key.D4;
                case Keys.D5: return NoesisLib.Key.D5;
                case Keys.D6: return NoesisLib.Key.D6;
                case Keys.D7: return NoesisLib.Key.D7;
                case Keys.D8: return NoesisLib.Key.D8;
                case Keys.D9: return NoesisLib.Key.D9;
                case Keys.A: return NoesisLib.Key.A;
                case Keys.B: return NoesisLib.Key.B;
                case Keys.C: return NoesisLib.Key.C;
                case Keys.D: return NoesisLib.Key.D;
                case Keys.E: return NoesisLib.Key.E;
                case Keys.F: return NoesisLib.Key.F;
                case Keys.G: return NoesisLib.Key.G;
                case Keys.H: return NoesisLib.Key.H;
                case Keys.I: return NoesisLib.Key.I;
                case Keys.J: return NoesisLib.Key.J;
                case Keys.K: return NoesisLib.Key.K;
                case Keys.L: return NoesisLib.Key.L;
                case Keys.M: return NoesisLib.Key.M;
                case Keys.N: return NoesisLib.Key.N;
                case Keys.O: return NoesisLib.Key.O;
                case Keys.P: return NoesisLib.Key.P;
                case Keys.Q: return NoesisLib.Key.Q;
                case Keys.R: return NoesisLib.Key.R;
                case Keys.S: return NoesisLib.Key.S;
                case Keys.T: return NoesisLib.Key.T;
                case Keys.U: return NoesisLib.Key.U;
                case Keys.V: return NoesisLib.Key.V;
                case Keys.W: return NoesisLib.Key.W;
                case Keys.X: return NoesisLib.Key.X;
                case Keys.Y: return NoesisLib.Key.Y;
                case Keys.Z: return NoesisLib.Key.Z;
                case Keys.LeftWindows: return NoesisLib.Key.LWin;
                case Keys.RightWindows: return NoesisLib.Key.RWin;
                case Keys.Apps: return NoesisLib.Key.Apps;
                case Keys.Sleep: return NoesisLib.Key.Sleep;
                case Keys.NumPad0: return NoesisLib.Key.NumPad0;
                case Keys.NumPad1: return NoesisLib.Key.NumPad1;
                case Keys.NumPad2: return NoesisLib.Key.NumPad2;
                case Keys.NumPad3: return NoesisLib.Key.NumPad3;
                case Keys.NumPad4: return NoesisLib.Key.NumPad4;
                case Keys.NumPad5: return NoesisLib.Key.NumPad5;
                case Keys.NumPad6: return NoesisLib.Key.NumPad6;
                case Keys.NumPad7: return NoesisLib.Key.NumPad7;
                case Keys.NumPad8: return NoesisLib.Key.NumPad8;
                case Keys.NumPad9: return NoesisLib.Key.NumPad9;
                case Keys.Multiply: return NoesisLib.Key.Multiply;
                case Keys.Add: return NoesisLib.Key.Add;
                case Keys.Separator: return NoesisLib.Key.Separator;
                case Keys.Subtract: return NoesisLib.Key.Subtract;
                case Keys.Decimal: return NoesisLib.Key.Decimal;
                case Keys.Divide: return NoesisLib.Key.Divide;
                case Keys.F1: return NoesisLib.Key.F1;
                case Keys.F2: return NoesisLib.Key.F2;
                case Keys.F3: return NoesisLib.Key.F3;
                case Keys.F4: return NoesisLib.Key.F4;
                case Keys.F5: return NoesisLib.Key.F5;
                case Keys.F6: return NoesisLib.Key.F6;
                case Keys.F7: return NoesisLib.Key.F7;
                case Keys.F8: return NoesisLib.Key.F8;
                case Keys.F9: return NoesisLib.Key.F9;
                case Keys.F10: return NoesisLib.Key.F10;
                case Keys.F11: return NoesisLib.Key.F11;
                case Keys.F12: return NoesisLib.Key.F12;
                case Keys.F13: return NoesisLib.Key.F13;
                case Keys.F14: return NoesisLib.Key.F14;
                case Keys.F15: return NoesisLib.Key.F15;
                case Keys.F16: return NoesisLib.Key.F16;
                case Keys.F17: return NoesisLib.Key.F17;
                case Keys.F18: return NoesisLib.Key.F18;
                case Keys.F19: return NoesisLib.Key.F19;
                case Keys.F20: return NoesisLib.Key.F20;
                case Keys.F21: return NoesisLib.Key.F21;
                case Keys.F22: return NoesisLib.Key.F22;
                case Keys.F23: return NoesisLib.Key.F23;
                case Keys.F24: return NoesisLib.Key.F24;
                case Keys.NumLock: return NoesisLib.Key.NumLock;
                case Keys.Scroll: return NoesisLib.Key.Scroll;
                case Keys.LeftShift: return NoesisLib.Key.LeftShift;
                case Keys.RightShift: return NoesisLib.Key.RightShift;
                case Keys.LeftControl: return NoesisLib.Key.LeftCtrl;
                case Keys.RightControl: return NoesisLib.Key.RightCtrl;
                case Keys.LeftAlt: return NoesisLib.Key.LeftAlt;
                case Keys.RightAlt: return NoesisLib.Key.RightAlt;
                case Keys.BrowserBack: return NoesisLib.Key.BrowserBack;
                case Keys.BrowserForward: return NoesisLib.Key.BrowserForward;
                case Keys.BrowserRefresh: return NoesisLib.Key.BrowserRefresh;
                case Keys.BrowserStop: return NoesisLib.Key.BrowserStop;
                case Keys.BrowserSearch: return NoesisLib.Key.BrowserSearch;
                case Keys.BrowserFavorites: return NoesisLib.Key.BrowserFavorites;
                case Keys.BrowserHome: return NoesisLib.Key.BrowserHome;
                case Keys.VolumeMute: return NoesisLib.Key.VolumeMute;
                case Keys.VolumeDown: return NoesisLib.Key.VolumeDown;
                case Keys.VolumeUp: return NoesisLib.Key.VolumeUp;
                case Keys.MediaNextTrack: return NoesisLib.Key.MediaNextTrack;
                case Keys.MediaPreviousTrack: return NoesisLib.Key.MediaPreviousTrack;
                case Keys.MediaStop: return NoesisLib.Key.MediaStop;
                case Keys.MediaPlayPause: return NoesisLib.Key.MediaPlayPause;
                case Keys.LaunchMail: return NoesisLib.Key.LaunchMail;
                case Keys.SelectMedia: return NoesisLib.Key.SelectMedia;
                case Keys.LaunchApplication1: return NoesisLib.Key.LaunchApplication1;
                case Keys.LaunchApplication2: return NoesisLib.Key.LaunchApplication2;
                case Keys.OemSemicolon: return NoesisLib.Key.OemSemicolon;
                case Keys.OemPlus: return NoesisLib.Key.OemPlus;
                case Keys.OemComma: return NoesisLib.Key.OemComma;
                case Keys.OemMinus: return NoesisLib.Key.OemMinus;
                case Keys.OemPeriod: return NoesisLib.Key.OemPeriod;
                case Keys.OemQuestion: return NoesisLib.Key.OemQuestion;
                case Keys.OemTilde: return NoesisLib.Key.OemTilde;
                case Keys.OemOpenBrackets: return NoesisLib.Key.OemOpenBrackets;
                case Keys.OemPipe: return NoesisLib.Key.OemPipe;
                case Keys.OemCloseBrackets: return NoesisLib.Key.OemCloseBrackets;
                case Keys.OemQuotes: return NoesisLib.Key.OemQuotes;
                case Keys.Oem8: return NoesisLib.Key.Oem8;
                case Keys.OemBackslash: return NoesisLib.Key.OemBackslash;
                case Keys.ProcessKey: return NoesisLib.Key.ImeProcessed;
                case Keys.OemCopy: return NoesisLib.Key.OemCopy;
                case Keys.OemAuto: return NoesisLib.Key.OemAuto;
                case Keys.OemEnlW: return NoesisLib.Key.OemEnlw;
                case Keys.Attn: return NoesisLib.Key.Attn;
                case Keys.Crsel: return NoesisLib.Key.CrSel;
                case Keys.Exsel: return NoesisLib.Key.ExSel;
                case Keys.EraseEof: return NoesisLib.Key.EraseEof;
                case Keys.Play: return NoesisLib.Key.Play;
                case Keys.Zoom: return NoesisLib.Key.Zoom;
                case Keys.Pa1: return NoesisLib.Key.Pa1;
                case Keys.OemClear: return NoesisLib.Key.OemClear;
                default: return NoesisLib.Key.None;
            }
        }
        
    }
}