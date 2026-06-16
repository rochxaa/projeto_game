# Projeto Jogo 2D Plataforma — Unity 6

Um jogo de plataforma 2D com combate estilo Hollow Knight, desenvolvido em Unity 6 com C#.

---

## Sobre o Jogo

Jogo de plataforma 2D com combate corpo a corpo. O jogador controla um personagem que pode se mover, pular (com pulo duplo), correr e atacar inimigos. O objetivo é sobreviver aos inimigos patrulheiros enquanto coleta corações para recuperar vida.

---

## Controles

| Tecla | Ação |
|---|---|
| `A` / `←` | Mover para esquerda |
| `D` / `→` | Mover para direita |
| `Espaço` / `↑` | Pular (até 2x seguidos) |
| `Shift` | Correr |
| `Botão esquerdo do mouse` | Atacar |

---

## Mecânicas do Jogo

### Personagem
- Movimentação lateral com WASD ou setas do teclado
- Pulo duplo
- Corrida ao segurar Shift
- Sistema de ataque corpo a corpo com animação
- Sprite vira na direção do movimento
- Gravidade com sensação arcade (pulo curto ao soltar rápido, pulo alto ao segurar)

### Sistema de Vida
- O personagem possui até 5 corações de vida
- Corações coletáveis aparecem no mapa
- Ao coletar um coração, o personagem recupera 1 ponto de vida
- Ao chegar a 0 de vida, a animação de morte é tocada

### Inimigo — Keeper
- Patrulha uma área fixa do mapa
- Possui campo de visão de 6 unidades
- Ao detectar o jogador, aumenta a velocidade e persegue
- Nunca sai da sua área de patrulha, mesmo ao perseguir
- Causa 1 ponto de dano ao tocar o jogador
- Possui 3 pontos de vida
- Ao morrer, toca animação de morte e some do mapa

---

## Estrutura do Projeto

```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── PlayerController.cs   ← movimento, pulo, ataque, animações
│   │   └── Life.cs               ← coração coletável
│   └── Enemies/
│       └── Keeper/
│           └── Keeper.cs         ← IA do inimigo patrulheiro
├── Animations/
│   ├── Player/
│   │   ├── idle
│   │   ├── run
│   │   ├── attack
│   │   └── die
│   └── Enemies/
│       └── Keeper/
│           ├── walk
│           ├── attack
│           └── death
├── Prefabs/
├── Scenes/
└── Images/
```

---

## Tecnologias Utilizadas

- **Unity 6**
- **C#**
- **Unity Input System** (novo sistema de input)
- **Cinemachine** (câmera que segue o personagem)
- **TextMeshPro** (UI de vida)
- **Rigidbody2D** com física 2D
- **Animator Controller** para animações


---

## Requisitos

- Sistema Operacional: Windows 10 ou superior
- Placa de Vídeo com suporte a DirectX 11
- 4GB de RAM

---

## Inimigos

| Inimigo | Vida | Dano | Velocidade | Campo de Visão |
|---|---|---|---|---|
| Keeper | 3 | 1 | 2 (patrulha) / 4 (perseguição) | 6 unidades |

---

## Configurações do Personagem (Inspector)

| Parâmetro | Valor Padrão |
|---|---|
| Speed | 5 |
| Run Speed | 10 |
| Jump Force | 12 |
| Max Jumps | 2 |
| Fall Multiplier | 3 |
| Low Jump Multiplier | 2 |

---

## Notas de Desenvolvimento

- O projeto foi desenvolvido utilizando o **novo Input System** do Unity 6
- O Rigidbody2D do personagem tem **Gravity Scale = 1** e **Freeze Rotation Z** ativado
- A detecção de chão utiliza **OnCollisionEnter2D** com a tag `Ground`
- O Keeper utiliza **GetComponentInChildren** para acessar o Animator no objeto filho `Skin`
- A área de patrulha do Keeper é visualizável no Editor pela linha amarela no **Gizmo**
