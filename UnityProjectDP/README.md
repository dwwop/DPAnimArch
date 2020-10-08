# AnimArch
Repozitár tímu ArchMages19.
## **Pravidlá práce s GitHub:**
1. V prípade, že člen tímu chce spraviť merge do upstream/master musí vytvoriť pull request, ktorý musí nechať skontrolovať iným členom tímu
2. Commit musí obsahovať správu, ktorá jasne vyjadruje zmysel daného commitu a čo sa zmení po aplikovaní daného commitu [návod](https://chris.beams.io/posts/git-commit/)
3. Master vetva v lokálnom repozitári by mala byť vždy zhodná s upstream/master vetvou

## **GitHub príkazy**
**Prístup do vzdialených repozitárov:**
```
git remote -v                                 - zobrazí názvy a url adresy vzdialených repozitárov
git remote add <názov remote> <adresa remote> - pridá prepojenie vzdialeného repozitára s menom <názov remote> pod url adresou <adresa remote>
git remote remove <názov remote>              - zmaže prepojenie vzdialeného repozitára s menom <názov remote>
git clone <adresa remote>                     - stiahne všetko zo vzdialeného repozitára s url <adresa remote> do lokálneho repozitára
```

**Správa vetiev:**
```
git branch                    - zobrazí lokálne vetvy
git branch -a                 - zobrazí všetky vetvy
git branch -D <názov vetvy>   - odstráni vetvu s menom <názov vetvy>
git checkout <názov vetvy>    - aktivuje vetvu s menom <názov vetvy>
git checkout -b <názov vetvy> - vytvorí novu vetvu s menom <názov vetvy> a aktivuje ju
```

**Správa lokálneho repozitára:**
```
git status                               - zobrazí aktuálny stav lokálneho repozitára oproti origin (viac info)
git checkout                             - zobrazí aktuálny stav  lokálneho repozitára oproti origin
git diff                                 - zobrazí rozdiely lokálneho repozitára oproti origin
git diff <názov vetvy>                   - zobrazí rozdiely lokálneho repozitára oproti vetve s menom <názov vetvy>
git diff <názov vetvy 1> <názov vetvy 2> - zobrazí rozdiely vetiev <názov vetvy 1> s vetvou <názov vetvy 2>
```

**Správou kódu lokálneho repozitára:**
```
git add <cesta súboru>              - pridá súbor do listu, ktoré budú commitnuté
git restore --staged <cesta súboru> - odstráni súbor z listu, ktoré majú byť commitnuté
git checkout <cesta súboru>         - obnoví súbor do pôvodného stavu
git commit -m "<message>"           - commitne súbory, ktoré sa nachádzajú v liste súborov, ktoré sa majú commitnúť, commit bude obsahovať správu <message>|
git push                            - pošle aktuálnu vetvu do tej istej vetvy, ale vo vzdialenom repozitári
git push -u origin <názov vetvy>    - vytvorí novú vetvu vo vzdialenom repozitári s menom <názov vetvy> a pošle tam aktuálnu vetvu
git pull                            - stiahne aktuálnu vetvu zo vzdialeného repozitára
git pull <názov vetvy>              - stiahne vetvu s menom <názov vetvy> zo vzdialeného repozitára
git stash                           - uloží zmeny do stash
git stash pop                       - vyberie posledné uložené zmeny zo stash a aplikuje ich
```

**GitHub príklad** (Stiahnutie najnovšej verzie kódu, pridanie novej funkcionality, aktualizovanie vzdialeného repozitára a vyžiadanie pull requestu)

1. git checkout master
2. git pull upstream/master
3. git checkout -b <feature/meno_novej_feature>
4. "Vykonanie zmien v kóde + pridanie nového súboru
5. git add --all
6. git commit -m "<Správa ktorá vyjadruje čo daný commit zmení v kóde>"
7. git push -u origin <feature/meno_novej_feature>
8. "Vytvorenie pull requestu na GitHub stránke, pre vetvu <feature/meno_novej_feature>"
