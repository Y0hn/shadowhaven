# Projekt
 skolsky projekt 2D RPG

 
___________________/ WORK \_____________________
Ciel:
	Vytvorit funkcne singleplayer 2D RPG s:
		combat systemom
			melee
			ranged
			magic
		equipment systemom
		moznostou hrania na ovladaci aj klavesnici
		dandznami (v jaskynach, budovach ...)
			v styte Binding of Issac
		interaktivnimi NPC

----------------< 03.10.  2023 >----------------
Zaciatok Dokumetacie

Pridane Funkcie: 
	pohyb hraca + kamery

Pridane Textury:
	Enviroment = { background, rock1, rock2, tree1, tree2 }
	Player = { torso, head, armL, armR, legL, legR }

Pridane Animacie:
	Player = { RunR, RunL, RunU, RunD, Idle}
		Movement(RunR, RunL, RunU, RunD)
	  Tanzisions:
		Idle => Movement
		Movement => Idle
		
Vytvorene Animatory:
	Player.Player
		Variables { float Horizontal, float Vertical, float speed}
		Tranzicion time (Movement <=> Idle) = 0.1


Pridane UI:
	Canvas + EventSystem

Pridane Sceny:
	InGame
	MainMenu

Pridane Scripty:
	PlayerScript
	GameManager
	MenuManager

----------------< 04.10.  2023 >----------------
Pridane Scripty:
	PlayerData
	SaveSystem

Upravene Scripty:
	PlayerSystem
	GameManager

Pridane Textury:
	Weapons = { sword, bow, knife }

Pridane Objekty:
	Player.body.armL.WeaponL
	Player.body.armR.WeaponR
	Canvas.PauseMenu

GitHub Commit:
	First Sample Commit

----------------< 05.10.  2023 >----------------
Pridane Funkcie:
	Save
	Load
	Pause
	Resume

Neuspesne Pokusy o:
	Pohyb PauseMenu mysou
	Load v PauseMenu (aby sa prejavil je potrebne z neho vystupit)

Upravene Scripty:
	GameManager
	PlayerScript

Pridane UI:
	Pause Menu = { resume-button, save-button, load-button, quit-button }

----------------< 06.10.  2023 >----------------
Pridane Funkcie:
	Attack
	Hit
	Die
	Enemy Random Health

Pridane Animacie:
	AttackL

Pridane Objekty:
	Player.body.armL.MeleeWeaponL.AttackPointL
	Player.body.armR.MeleeWeaponR.AttackPointR
		+ render ich range v scripte

Pridane Objekty:
	Enemy

Pridane Layery:
	Enemy

Pridane Scripty:
	EnemyScript

Upravene Animacie:
	Player.{ RunR, RunL, RunU, RunD }
		pridanie animacie pre MeleeWeapons

Uprava Animatory:
	Player.Player
		Tranzicion time (Movement <=> Idle) = 0
		pridane Variables { Trigger Attack, bool Melee, bool Ranged }

Upravene Objekty:
	WeaponL	=> MeleeWeaponL
	WeaponR	=> MeleeWeaponR

Upravene Scripty:
	PlayerScript

