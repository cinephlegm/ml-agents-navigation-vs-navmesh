# ML-Agents Navigation vs NavMesh

Research Design I project by **Josef Carl Muscat** comparing learned navigation in Unity ML-Agents with a conventional Unity NavMesh baseline.

## Study

The experiment trains a raycast-based agent with Proximal Policy Optimization (PPO) to navigate a constrained indoor environment with hallways, doorways and static obstacles.

The learned agent is compared against a conventional pathfinding baseline using Unity's `NavMeshAgent`.

The submitted report describes that baseline as A*. In the implementation, [`AStarAgent.cs`](unity/Assets/Scripts/AStarAgent.cs) delegates path planning to Unity's NavMesh system rather than implementing A* directly. This repository therefore refers to it as the **NavMesh baseline**.

## Learned agent

[`HouseAgent.cs`](unity/Assets/Agents/HouseAgent.cs) uses:

- **8 scalar observations**
  - goal direction on X/Z
  - normalised goal distance
  - five obstacle ray distances at -90°, -45°, 0°, 45° and 90°
- **2 continuous actions**
  - forward/backward movement
  - turning
- shaped reward based on progress toward the goal
- a small per-step penalty
- collision penalties
- a terminal reward for reaching the goal

The older discrete `NavigationAgent` prototype is intentionally not included.

## Training

| Parameter | Value |
| --- | ---: |
| Algorithm | PPO |
| Batch size | 2048 |
| Buffer size | 20480 |
| Learning rate | 5e-4 |
| Entropy coefficient | 0.001 |
| PPO clip epsilon | 0.2 |
| Lambda | 0.95 |
| Epochs | 3 |
| Hidden units | 256 |
| Hidden layers | 2 |
| Gamma | 0.99 |
| Time horizon | 1000 |
| Maximum steps | 5,000,000 |

The exact configuration is in [`training/config.yaml`](training/config.yaml).

## Evaluation

The submitted study evaluated the learned policy using **success rate, path length and learning progression**, with the NavMesh baseline used for comparison.

The report concluded that reward-shaped PPO could learn effective navigation in the tested structured environment. This was a small controlled prototype, not a general claim that reinforcement learning outperforms classical pathfinding.

## Repository

```text
training/   final PPO trainer configuration
unity/      final agent, NavMesh baseline, environment spawner and package metadata
paper/      final Research Design I report
```

The original coursework repository also contained milestone archives, downloaded literature, presentation files, a prototype video and a third-party `LowPolyInterior` environment. Those are not redistributed here.

## Environment

- Unity **2022.3.49f1**
- Unity ML-Agents **2.0.1**
- Unity AI Navigation **1.1.6**

## Key references

- A. Juliani et al., [*Unity: A General Platform for Intelligent Agents*](https://arxiv.org/abs/1809.02627), 2020.
- H. Qiu, [*Multi-agent navigation based on deep reinforcement learning and traditional pathfinding algorithm*](https://arxiv.org/abs/2012.09134), 2020.
- A. Nash, A. Vardy and D. Churchill, [*Herd's Eye View: Improving Game AI Agent Learning with Collaborative Perception*](https://doi.org/10.1609/aiide.v19i1.27526), AIIDE 2023.
- [Unity ML-Agents Toolkit](https://github.com/Unity-Technologies/ml-agents).
- [Unity `NavMeshAgent` API](https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent.html).

## Citation

If you use this repository, cite the project:

```text
J. C. Muscat, "ML-Agents Navigation vs NavMesh," Research Design I project, MCAST Institute of Information and Communication Technology, 2025.
```
