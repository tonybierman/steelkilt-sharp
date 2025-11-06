using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SteelkiltSharp.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SteelkiltMonoGame
{
    public class Game3 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _regularFont;
        private SpriteFont _largeFont;
        private SpriteFont _monoFont;

        private List<Character> _characters;
        private List<string> _combatLog;
        private bool _isExecuting;
        private bool _canReset;
        private int _combatLogScrollPosition;
        private int _previousScrollValue;

        private Rectangle _executeCombatButtonRect;
        private Rectangle _resetButtonRect;
        private Rectangle _combatLogRect;
        private MouseState _previousMouseState;

        private const int SCREEN_WIDTH = 1400;
        private const int SCREEN_HEIGHT = 900;
        private const int LINE_HEIGHT = 21;
        private const int LOG_HEIGHT = 400;
        private const int VISIBLE_LINES = 18;

        public Game3()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;

            _characters = new List<Character>();
            _combatLog = new List<string>();
            _isExecuting = false;
            _canReset = false;
            _combatLogScrollPosition = 0;
            _previousScrollValue = 0;
        }

        protected override void Initialize()
        {
            base.Initialize();
            InitializeCharacters();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _regularFont = Content.Load<SpriteFont>("Score");
            _largeFont = Content.Load<SpriteFont>("Score");
            _monoFont = Content.Load<SpriteFont>("Score");
        }

        private void InitializeCharacters()
        {
            _characters.Clear();

            var elaraAttributes = new Attributes(7, 8, 6, 6, 7, 6, 7, 8, 7);
            var elaraWeapon = new Weapon("Longsword", WeaponImpact.Medium);
            var elaraArmor = new Armor("Steel Plate", ArmorType.FullPlate, 8, 2);
            var elara = new Character("Elara Sunblade", elaraAttributes, 7, 6, elaraWeapon, elaraArmor);
            _characters.Add(elara);

            var aldricAttributes = new Attributes(8, 6, 7, 7, 6, 8, 6, 6, 6);
            var aldricWeapon = new Weapon("Great Axe", WeaponImpact.Large);
            var aldricArmor = new Armor("Chain Mail", ArmorType.Chain, 6, 1);
            var aldric = new Character("Aldric the Bold", aldricAttributes, 8, 4, aldricWeapon, aldricArmor);
            _characters.Add(aldric);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var mouseState = Mouse.GetState();

            // Handle scrolling when mouse is over combat log
            if (_combatLogRect.Contains(mouseState.Position) && _combatLog.Count > VISIBLE_LINES)
            {
                int scrollDelta = mouseState.ScrollWheelValue - _previousScrollValue;
                if (scrollDelta != 0)
                {
                    // Scroll by 3 lines per wheel notch
                    int scrollLines = scrollDelta / 120 * 3;
                    _combatLogScrollPosition -= scrollLines;

                    // Clamp scroll position
                    int maxScroll = Math.Max(0, _combatLog.Count - VISIBLE_LINES);
                    _combatLogScrollPosition = Math.Clamp(_combatLogScrollPosition, 0, maxScroll);
                }
                _previousScrollValue = mouseState.ScrollWheelValue;
            }

            // Handle button clicks
            if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                if (_executeCombatButtonRect.Contains(mouseState.Position) && !_isExecuting)
                {
                    ExecuteCombatRounds();
                }

                if (_resetButtonRect.Contains(mouseState.Position) && _canReset && !_isExecuting)
                {
                    ResetCombat();
                }
            }

            _previousMouseState = mouseState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();

            DrawUI();

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawUI()
        {
            int yPos = 20;
            int xMargin = 20;
            int columnWidth = 400;

            _spriteBatch.DrawString(_largeFont, "Steelkilt Sharp Combat System",
                new Vector2(xMargin, yPos), Color.Black);
            yPos += 60;

            DrawCombatLogColumn(xMargin, yPos, columnWidth - 20);

            for (int i = 0; i < _characters.Count; i++)
            {
                int xPos = xMargin + columnWidth + (i * columnWidth);
                DrawCharacterCard(_characters[i], xPos, yPos, columnWidth - 20);
            }
        }

        private void DrawCombatLogColumn(int x, int y, int width)
        {
            _executeCombatButtonRect = new Rectangle(x, y, width, 40);
            DrawButton(x, y, width, 40, "Execute Combat Rounds", !_isExecuting, null);
            y += 50;

            if (_combatLog.Count > 0)
            {
                _spriteBatch.DrawString(_regularFont, "Combat Log (scroll with mouse wheel)",
                    new Vector2(x, y), Color.Black);
                y += 30;

                _combatLogRect = new Rectangle(x, y, width, LOG_HEIGHT);
                DrawRectangle(x, y, width, LOG_HEIGHT, Color.Black);
                DrawRectangle(x + 2, y + 2, width - 4, LOG_HEIGHT - 4, Color.FromNonPremultiplied(30, 30, 30, 255));

                // Draw combat log lines with scrolling
                int logY = y + 10;
                int startLine = _combatLogScrollPosition;
                int endLine = Math.Min(startLine + VISIBLE_LINES, _combatLog.Count);

                for (int i = startLine; i < endLine; i++)
                {
                    _spriteBatch.DrawString(_monoFont, _combatLog[i],
                        new Vector2(x + 10, logY), Color.Lime);
                    logY += LINE_HEIGHT;
                }

                // Draw scrollbar if needed
                if (_combatLog.Count > VISIBLE_LINES)
                {
                    DrawScrollbar(x + width - 14, y + 2, 12, LOG_HEIGHT - 4);
                }

                y += LOG_HEIGHT + 20;
            }

            if (_canReset)
            {
                _resetButtonRect = new Rectangle(x, y, width, 40);
                DrawButton(x, y, width, 40, "Reset", !_isExecuting, null);
            }
        }

        private void DrawScrollbar(int x, int y, int width, int height)
        {
            // Scrollbar background
            DrawRectangle(x, y, width, height, Color.DarkGray);

            // Calculate thumb size and position
            float contentRatio = (float)VISIBLE_LINES / _combatLog.Count;
            int thumbHeight = Math.Max(20, (int)(height * contentRatio));

            float scrollRatio = (float)_combatLogScrollPosition / Math.Max(1, _combatLog.Count - VISIBLE_LINES);
            int thumbY = y + (int)((height - thumbHeight) * scrollRatio);

            // Scrollbar thumb
            DrawRectangle(x, thumbY, width, thumbHeight, Color.Gray);
        }

        private void DrawCharacterCard(Character character, int x, int y, int width)
        {
            DrawRectangle(x, y, width, 750, Color.LightGray);
            DrawRectangle(x + 2, y + 2, width - 4, 746, Color.White);

            int currentY = y + 10;

            DrawRectangle(x + 2, currentY, width - 4, 35, Color.CornflowerBlue);
            _spriteBatch.DrawString(_largeFont, character.Name,
                new Vector2(x + 10, currentY + 5), Color.White);
            currentY += 50;

            _spriteBatch.DrawString(_regularFont, "Wounds",
                new Vector2(x + 10, currentY), Color.Black);
            currentY += 25;

            string woundsText = $"Light: {character.Wounds.Light}  Severe: {character.Wounds.Severe}  Critical: {character.Wounds.Critical}";
            _spriteBatch.DrawString(_regularFont, woundsText,
                new Vector2(x + 15, currentY), Color.Black);
            currentY += 25;

            Color statusColor = character.IsDead ? Color.Red : Color.Green;
            string statusText = character.IsDead ? "DEAD" : "ALIVE";
            _spriteBatch.DrawString(_regularFont, $"Status: {statusText}",
                new Vector2(x + 15, currentY), statusColor);
            currentY += 35;

            _spriteBatch.DrawString(_regularFont, "Attributes",
                new Vector2(x + 10, currentY), Color.Black);
            currentY += 25;

            string attributesText = $"STR: {character.Attributes.Strength}  DEX: {character.Attributes.Dexterity}  CON: {character.Attributes.Constitution}\n" +
                                   $"REA: {character.Attributes.Reason}  INT: {character.Attributes.Intuition}  WIL: {character.Attributes.Willpower}\n" +
                                   $"CHA: {character.Attributes.Charisma}  PER: {character.Attributes.Perception}  EMP: {character.Attributes.Empathy}";

            foreach (string line in attributesText.Split('\n'))
            {
                _spriteBatch.DrawString(_regularFont, line,
                    new Vector2(x + 15, currentY), Color.Black);
                currentY += 20;
            }
            currentY += 20;

            _spriteBatch.DrawString(_regularFont, "Skills",
                new Vector2(x + 10, currentY), Color.Black);
            currentY += 25;

            _spriteBatch.DrawString(_regularFont, $"Weapon Skill: {character.WeaponSkill}",
                new Vector2(x + 15, currentY), Color.Black);
            currentY += 20;
            _spriteBatch.DrawString(_regularFont, $"Dodge Skill: {character.DodgeSkill}",
                new Vector2(x + 15, currentY), Color.Black);
            currentY += 30;

            _spriteBatch.DrawString(_regularFont, "Equipment",
                new Vector2(x + 10, currentY), Color.Black);
            currentY += 25;

            _spriteBatch.DrawString(_regularFont, $"Weapon: {character.Weapon.Name} ({character.Weapon.Impact})",
                new Vector2(x + 15, currentY), Color.Black);
            currentY += 20;
            _spriteBatch.DrawString(_regularFont, $"Armor: {character.Armor.Name} ({character.Armor.ArmorType})",
                new Vector2(x + 15, currentY), Color.Black);
            currentY += 20;
            _spriteBatch.DrawString(_regularFont, $"Protection: {character.Armor.Protection}, Penalty: {character.Armor.MovementPenalty}",
                new Vector2(x + 15, currentY), Color.Black);
        }

        private void DrawButton(int x, int y, int width, int height, string text, bool enabled, Color? bgColor = null)
        {
            Color borderColor = enabled ? Color.Black : Color.Gray;
            Color backgroundColor = bgColor ?? (enabled ? Color.CornflowerBlue : Color.LightGray);
            Color textColor = enabled ? Color.White : Color.DarkGray;

            DrawRectangle(x, y, width, height, borderColor);
            DrawRectangle(x + 2, y + 2, width - 4, height - 4, backgroundColor);

            Vector2 textSize = _regularFont.MeasureString(text);
            Vector2 textPos = new Vector2(
                x + (width - textSize.X) / 2,
                y + (height - textSize.Y) / 2
            );

            _spriteBatch.DrawString(_regularFont, text, textPos, textColor);
        }

        private void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            Texture2D rect = new Texture2D(GraphicsDevice, 1, 1);
            rect.SetData(new[] { color });
            _spriteBatch.Draw(rect, new Rectangle(x, y, width, height), color);
        }

        private async Task ExecuteCombatRounds()
        {
            if (_characters == null || _characters.Count < 2) return;

            _isExecuting = true;
            _combatLog.Clear();

            var combatant1 = _characters[0];
            var combatant2 = _characters[1];

            int i = 0;
            while (true)
            {
                _combatLog.Add($"Round {++i}:");

                var d10Roll1 = Dice.D10();
                var d10Roll2 = Dice.D10();
                bool coinFlip = d10Roll1 + combatant1.Attributes.Dexterity > d10Roll2 + combatant2.Attributes.Dexterity;
                Character attacker = coinFlip ? combatant1 : combatant2;
                Character defender = attacker == combatant1 ? combatant2 : combatant1;

                var result1 = Combat.CombatRound(attacker, defender, DefenseAction.Parry);
                _combatLog.Add($"{result1}");
                _combatLog.Add($"  {defender.Name} wounds: {defender.Wounds}");
                defender.OnPropertyChanged(nameof(Character.Wounds));
                defender.OnPropertyChanged(nameof(Character.Exhaustion));
                _combatLog.Add("");

                if (!defender.IsDead)
                {
                    var result2 = Combat.CombatRound(defender, attacker, DefenseAction.Parry);
                    _combatLog.Add($"{result2}");
                    _combatLog.Add($"  {attacker.Name} wounds: {attacker.Wounds}");
                    attacker.OnPropertyChanged(nameof(Character.Wounds));
                    attacker.OnPropertyChanged(nameof(Character.Exhaustion));
                    _combatLog.Add("");
                }

                if (attacker.IsDead || defender.IsDead || i > 40)
                {
                    _combatLog.Add("=== Combat ended ===");
                    if (attacker.IsDead)
                        _combatLog.Add($"{attacker.Name} has been defeated!");
                    else if (defender.IsDead)
                        _combatLog.Add($"{defender.Name} has been defeated!");
                    else
                        _combatLog.Add("Combat reached round limit.");

                    break;
                }

                await Task.Delay(1000);
            }

            _isExecuting = false;
            _canReset = true;
        }

        private void ResetCombat()
        {
            _combatLog.Clear();
            InitializeCharacters();
            _canReset = false;
            _combatLogScrollPosition = 0;
        }
    }
}