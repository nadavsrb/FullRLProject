import random
import sys
import ast
import operator
import numpy as np

disc_rate = None
epsilon_rate = None
learning_rate = None
max_episodes = None
max_steps = None
rewards = None
states_data = None
start_pos = None


def get_initials(file_name):
    global disc_rate, epsilon_rate, learning_rate, \
        max_episodes, max_steps, rewards, states_data, start_pos

    file_path = f"{sys.argv[1]}/{file_name}"
    with open(file_path, 'r') as file:
        while line := file.readline():
            line = line.strip()

            if line == "discount rate:":
                disc_rate = float(file.readline().strip())
            elif line == "epsilon rate:":
                epsilon_rate = tuple(map(float, file.readline().strip().split(' ')))
            elif line == "learning rate:":
                learning_rate = float(file.readline().strip())
            elif line == "max episodes:":
                max_episodes = int(file.readline().strip())
            elif line == "max steps:":
                max_steps = int(file.readline().strip())
            elif line == "rewards by state-type:":
                rewards = ast.literal_eval(file.readline().strip())
            elif line == "states data:":
                data_str = []
                while new_line := file.readline().strip():
                    data_str.append(np.array(new_line.split(' ')))
                states_data = np.array(data_str)
            elif line == "starting position:":
                start_pos = tuple(map(int, file.readline().strip().split(' ')))


def ql_learn_value_iter(out_episodes_file, out_final_qtable):
    qtable = np.zeros(
        (states_data.shape[0], states_data.shape[1], 4))  # the actions are: {up:0, right:1, down:2, left:3}
    epsilon = epsilon_rate[0]

    action_offset = [(-1, 0), (0, 1), (1, 0), (0, -1)]  # up, right, down, left

    for episode in range(max_episodes):
        pos = start_pos
        episode_actions = []
        episode_reword = 0

        rand_num = random.uniform(0, 1)
        should_explore = True
        if rand_num > epsilon:
            should_explore = False  # we will exploit instead

        is_game_over = False
        num_steps = 0
        while (not is_game_over) and (num_steps < max_steps):
            action = None
            if should_explore:
                action = random.randint(0, 3)
            else:
                action = np.argmax(qtable[pos[0], pos[1]])

            episode_actions.append(action)

            new_pos = tuple(map(operator.add, pos, action_offset[action]))

            res_state = 'o'  # we assume we aren't in range of the board
            is_game_over = True
            max_expec_reward = 0
            if (new_pos[0] in range(states_data.shape[0])) and (new_pos[1] in range(states_data.shape[1])):
                res_state = states_data[new_pos[0], new_pos[1]]

                if res_state not in ['o', 'b', 't']:
                    is_game_over = False
                    max_expec_reward = np.amax(qtable[new_pos[0], new_pos[1]])

            res_state_reward = rewards[res_state]
            episode_reword += res_state_reward

            qtable[pos[0], pos[1], action] = (1 - learning_rate) * qtable[pos[0], pos[1], action] + \
                                             learning_rate * (res_state_reward + disc_rate * max_expec_reward)

            num_steps += 1
            pos = new_pos

        #  updating epsilon by it's rate:
        epsilon = (epsilon_rate[0] - epsilon_rate[1]) * (1 - (episode + 1) / max_episodes) + epsilon_rate[1]

        print(episode_actions)


def main():
    get_initials("ql_data.txt")

    ql_learn_value_iter("ql_out_episodes.txt", "ql_out_qtable.txt")


if __name__ == '__main__':
    main()
