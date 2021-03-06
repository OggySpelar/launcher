﻿using MetroFramework.Controls;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System;

namespace ResourceHubLauncher {

    enum ModButtonStates {
        Installed,
        Disabled,
        Available
    }

    class ModButton : MetroPanel {
        public string modName;
        Color modNameColor;
        string modSafety;
        Color modSafetyColor;
        string modState;
        Color modStateColor;
        public bool fromOutside = false;
        public ModButtonStates State;

        protected Font font = new Font("Segoe UI Light", 10f);

        Action<string> clickR;
        Action<string> hoverR;

        public ModButton(string _modName, int _modSafety, ModButtonStates _modState, Action<string> clickResult, Action<string> hoverResult) {
            modNameColor = Color.FromArgb(170, 170, 170);
            Size = new Size(177, 88);
            modName = _modName;
            BorderStyle = BorderStyle.FixedSingle;
            switch (_modState) {
                case ModButtonStates.Available:
                    modState = "Available";
                    modStateColor = Color.DodgerBlue;
                    break;
                case ModButtonStates.Disabled:
                    modState = "Disabled";
                    modStateColor = Color.Red;
                    break;
                case ModButtonStates.Installed:
                    modState = "Installed";
                    modStateColor = Color.Green;
                    break;
                default:
                    break;
            }

            switch (_modSafety) {
                case -1:
                    modSafety = "Inapplicable";
                    modSafetyColor = Color.FromArgb(170, 170, 170);
                    break;
                case 0:
                    modSafety = "Safe";
                    modSafetyColor = Color.Green;
                    break;
                case 1:
                    modSafety = "Medium";
                    modSafetyColor = Color.Orange;
                    break;
                case 2:
                    modSafety = "Unsafe";
                    modSafetyColor = Color.OrangeRed;
                    break;
                case 3:
                    modSafety = "Dangerous";
                    modSafetyColor = Color.Red;
                    break;
                default:
                    modSafety = "N/A";
                    modSafetyColor = Color.FromArgb(170, 170, 170);
                    break;
            }

            setLocation(new Point(0, 0));
            BringToFront();

            clickR = clickResult;
            hoverR = hoverResult;

            MouseDown += button1_Click;
            MouseHover += MouseHover_;

            State = _modState;

            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }



        public void setLocation(Point newLocation) {
            Location = newLocation;
        }

        override protected void OnPaintBackground(PaintEventArgs e) { }

        override protected void OnPaint(PaintEventArgs e) {
            using (Bitmap bmp = new Bitmap(Width, Height)) {
                using (Graphics g = Graphics.FromImage(bmp)) {
                    g.Clear(BackColor);
                    SolidBrush brush = new SolidBrush(modNameColor);
                    g.DrawString(modName, font, brush, new Point(10, 9));
                    brush = new SolidBrush(modStateColor);
                    g.DrawString(modState, font, brush, new Point(10, 59));
                    brush = new SolidBrush(modSafetyColor);
                    SizeF safetySize = g.MeasureString(modSafety, font);
                    g.DrawString(modSafety, font, brush, new Point(Size.Width - ((int)safetySize.Width) - 11, 59));
                    brush.Dispose();
                }
                e.Graphics.DrawImage(bmp, new Point(0, 0));
            }
        }

        public bool InQueue {
            set {
                if (value) {
                    modState = "In Queue";
                } else {
                    switch (State) {
                        case ModButtonStates.Available:
                            modState = "Available";
                            break;
                        case ModButtonStates.Disabled:
                            modState = "Disabled";
                            break;
                        case ModButtonStates.Installed:
                            modState = "Installed";
                            break;
                    }
                }
            }
            get { return modState == "In Queue"; }
        }

        public bool InstalledMod {
            get { return State == ModButtonStates.Installed; }
            set {
                if (value) {
                    State = ModButtonStates.Installed;
                    modState = "Installed";
                    modStateColor = Color.Green;
                } else {
                    State = ModButtonStates.Available;
                    modState = "Available";
                    modStateColor = Color.DodgerBlue;
                }
            }
        }

        public bool AvailableMod {
            get { return State == ModButtonStates.Available; }
            set {
                if (!value) {
                    State = ModButtonStates.Installed;
                    modState = "Installed";
                    modStateColor = Color.Green;
                } else {
                    State = ModButtonStates.Available;
                    modState = "Available";
                    modStateColor = Color.DodgerBlue;
                }
            }
        }

        public bool DisabledMod {
            get { return State == ModButtonStates.Disabled; }
            set {
                if (!value) {
                    State = ModButtonStates.Installed;
                    modState = "Installed";
                    modStateColor = Color.Green;
                } else {
                    State = ModButtonStates.Disabled;
                    modState = "Disabled";
                    modStateColor = Color.Red;
                }
            }
        }

        private void button1_Click(object sender, System.EventArgs e) {
            clickR(modName);

            ContextMenuStrip.Show(Cursor.Position);

        }

        private void MouseHover_(object sender, System.EventArgs e) {
            hoverR(modName);
        }

        public void ThemeChanged(bool lightTheme) {
            if (modSafetyColor == Color.FromArgb(17, 17, 17) || modSafetyColor == Color.FromArgb(170, 170, 170)) {
                if (lightTheme) {
                    modSafetyColor = Color.FromArgb(17, 17, 17);
                } else {
                    modSafetyColor = Color.FromArgb(170, 170, 170);

                }
            }


            if (lightTheme) {
                modNameColor = Color.FromArgb(17, 17, 17);
            } else {

                modNameColor = Color.FromArgb(170, 170, 170);
            }
        }



        public void changeContextMenu(ContextMenuStrip cMS) {
            ContextMenuStrip = cMS;
        }
    }

    class ModButtonList {
        public List<ModButton> list;
        List<int> elementsRemoved = new List<int>();
        Point latestAddedPos = new Point(0, -88);
        public ModButtonList() {
            list = new List<ModButton>();
        }

        public List<ModButton> GetMods() {
            return list;
        }

        public void Add(ModButton mod) {
            mod.setLocation(new Point(0, latestAddedPos.Y + 88));
            latestAddedPos = new Point(latestAddedPos.X, latestAddedPos.Y + 88);
            list.Add(mod);

        }

        public void Remove(string modName) {
            int index = list.FindIndex(mod => mod.modName == modName);
            elementsRemoved.Add(index);
        }

        public void ShowOnly(Func<ModButton, bool> how) {
            int actualMemberListN = 0;
            for (int i = 0; i < list.Count; i++) {
                if (how(list[i]) && elementsRemoved.FindIndex((m) => { return m == i; }) == -1) {
                    list[i].Visible = true;
                    list[i].setLocation(new Point(latestAddedPos.X, 88 * actualMemberListN));
                    actualMemberListN++;
                } else {
                    list[i].Visible = false;
                }
            }
            latestAddedPos = new Point(latestAddedPos.X, 0 - 88 + actualMemberListN * 88);
        }

        public void Clear() {
            latestAddedPos = new Point(0, -88);
            list.Clear();
        }

        public ModButton Find(string modName) {
            return list.Find(mod => mod.modName == modName);
        }

        public void ThemeChanged(bool lightTheme) {
            foreach (ModButton button in list) {
                button.ThemeChanged(lightTheme);
            }
        }
    }
}