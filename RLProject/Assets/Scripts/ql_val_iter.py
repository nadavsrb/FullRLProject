import random
import sys
import ast
import operator
import numpy as np
import os

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

    with open(file_name, 'r') as file:
        while line := file.readline():
            line = line.strip()

            if line == "discount rate:":
                disc_rate = float(file.readline().strip())
            elif line == "epsilon rate:":
                epsilon_rate = tuple(map(float, file.readline().strip().split(' ')))
            elif line == "learning rate:":
                learning_rate = tuple(map(float, file.readline().strip().split(' ')))
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


def linear_decrease(start, end):  # episodes run from 0 to max_episodes-1
    def func(episode):
        value = start
        if max_episodes != 1:
            value = (start - end) * (1 - episode / (max_episodes - 1)) + end
        return value

    return func


# (1-episode/(max_episodes - 1))^{0.5} deacreas from 1 to 0
def my_decrease():  # episodes run from 0 to max_episodes-1
    def func(episode):
        value = 1
        if max_episodes != 1:
            value = pow(1 - (episode / (max_episodes - 1)), 0.5)
        return value

    return func


def my_decrease2(pers_for_explore):  # episodes run from 0 to max_episodes-1
    def func(episode):
        value = 1
        if episode > pers_for_explore*max_episodes:
            value = 0
        return value

    return func


def ql_learn_value_iter(out_episodes_file, out_qtable_file):
    global states_data
    targs = list(map(tuple, np.argwhere(states_data == 't')))
    num_targs = len(targs)

    targs = {k: v for v, k in enumerate(targs)}

    bin_repres_targs = {k: 1 << v for k, v in targs.items()}
    bin_all_flags = pow(2, num_targs) - 1

    # the actions are: {up:0, right:1, down:2, left:3}
    qtable = np.zeros(
        (states_data.shape[0], states_data.shape[1], bin_all_flags + 1, 4))

    action_offset = [(-1, 0), (0, 1), (1, 0), (0, -1)]  # up, right, down, left

    epsilon_by_episode = linear_decrease(epsilon_rate[0], epsilon_rate[1])
    learning_by_episode = linear_decrease(learning_rate[0], learning_rate[1])

    with open(out_episodes_file, 'w') as out_episodes:

        for episode in range(max_episodes):
            pos = start_pos
            states_data = np.where(states_data == 'r', 't', states_data)
            episode_actions = []
            episode_reword = 0

            #  updating epsilon by it's rate:
            epsilon = epsilon_by_episode(episode)
            learning = learning_by_episode(episode)

            rand_num = random.uniform(0, 1)
            should_explore = True
            if rand_num > epsilon:
                should_explore = False  # we will exploit instead

            is_game_over = False
            num_steps = 0
            targs_reach = 0
            while (not is_game_over) and (num_steps < max_steps):
                if should_explore:
                    action = random.randint(0, 3)
                else:
                    action = np.argmax(qtable[pos[0], pos[1], targs_reach])

                episode_actions.append(action)

                new_pos = tuple(map(operator.add, pos, action_offset[action]))

                res_state = 'o'  # we assume we aren't in range of the board
                is_game_over = True
                max_expec_reward = 0
                new_targs_reach = targs_reach
                if (new_pos[0] in range(states_data.shape[0])) and (new_pos[1] in range(states_data.shape[1])):
                    res_state = states_data[new_pos[0], new_pos[1]]
                    if res_state == 't':
                        new_targs_reach += bin_repres_targs.get((new_pos[0], new_pos[1]))
                        states_data[new_pos[0], new_pos[1]] = 'r'

                    if (res_state not in ['o', 'b']) and (new_targs_reach != bin_all_flags):
                        is_game_over = False
                        max_expec_reward = np.amax(qtable[new_pos[0], new_pos[1], new_targs_reach])

                res_state_reward = rewards[res_state]
                episode_reword += res_state_reward

                qtable[pos[0], pos[1], targs_reach, action] = (1 - learning) * qtable[pos[0], pos[1],
                                                                                      targs_reach, action] + learning * (
                                                                          res_state_reward + disc_rate * max_expec_reward)

                targs_reach = new_targs_reach
                num_steps += 1
                pos = new_pos

            out_episodes.write(f"{'*' * 7} episode {episode + 1} {'*' * 7}\n"
                               f"episode actions: {episode_actions}\n"
                               f"episode reward: {episode_reword}\n\n")

    np.save(out_qtable_file, qtable)


def main():
    os.chdir(sys.argv[1])

    get_initials("ql_data.txt")

    ql_learn_value_iter("ql_out_episodes.txt", "ql_out_qtable")


if __name__ == '__main__':
    main()
