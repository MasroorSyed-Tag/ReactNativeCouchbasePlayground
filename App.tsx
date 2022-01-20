/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * Generated with the TypeScript template
 * https://github.com/react-native-community/react-native-template-typescript
 *
 * @format
 */

import React, {useEffect, useState} from 'react';
import {
  KeyboardAvoidingView,
  StyleSheet,
  Text,
  View,
  TextInput,
  TouchableOpacity,
  Keyboard,
  ScrollView,
  Platform,
} from 'react-native';
import Task from './components/Task';
import { NativeModules, NativeEventEmitter } from 'react-native';
import { Todo } from './Interfaces/Todo.model';

const TodoEventEmitter = new NativeEventEmitter(NativeModules.TodoItemRepository);
const TodoItemRepository = NativeModules.TodoItemRepository;

export default function App() {
  const [task, setTask] = useState<string | undefined>('');
  const [taskItems, setTaskItems] = useState<Todo[]>([]);

  useEffect(() => {
    TodoEventEmitter.addListener('SaveEvent', fetchTodo);
    fetchTodo();
  }, []);

  const fetchTodo = () => {
    console.log("inside fetch todo");
    TodoItemRepository.getTodos( function (result: Todo[]) {
      console.log("fetching");
      console.log(result);
      setTaskItems(result);
    });
  }


  const handleAddTask = () => {
    Keyboard.dismiss();
    TodoItemRepository.save(generateId(), task, function () {
      console.log("Saving");
      });
    // @ts-ignore
    setTask(null);
  };

  const completeTask = (id: string) => {

    TodoItemRepository.deleteTodos( id, function () {
      console.log("after del");
      fetchTodo();
    });
    // setTaskItems(itemsCopy);
  };

  const generateId = () => {
    const characters ='ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let result = ' ';
    const charactersLength = characters.length;
    const length = 10;
    for ( let i = 0; i < length; i++ ) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }

    return result;
  }

  return (
    <View style={styles.container}>
      {/* Added this scroll view to enable scrolling when list gets longer than the page */}
      <ScrollView
        contentContainerStyle={{
          flexGrow: 1,
        }}
        keyboardShouldPersistTaps="handled">
        {/* Today's Tasks */}
        <View style={styles.tasksWrapper}>
          <Text style={styles.sectionTitle}>Today's tasks</Text>
          <View style={styles.items}>
            {/* This is where the tasks will go! */}
            {taskItems?.map((item, index) => {
              return (
                <TouchableOpacity
                  key={index}
                  onPress={() => completeTask(item.Id)}>
                  <Task text={item.Description} />
                </TouchableOpacity>
              );
            })}
          </View>
        </View>
      </ScrollView>

      {/* Write a task */}
      {/* Uses a keyboard avoiding view which ensures the keyboard does not cover the items on screen */}
      <KeyboardAvoidingView
        behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
        style={styles.writeTaskWrapper}>
        <TextInput
          style={styles.input}
          placeholder={'Write a task'}
          value={task}
          onChangeText={text => setTask(text)}
        />
        <TouchableOpacity onPress={() => handleAddTask()}>
          <View style={styles.addWrapper}>
            <Text style={styles.addText}>+</Text>
          </View>
        </TouchableOpacity>
      </KeyboardAvoidingView>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#E8EAED',
  },
  tasksWrapper: {
    paddingTop: 80,
    paddingHorizontal: 20,
  },
  sectionTitle: {
    fontSize: 24,
    fontWeight: 'bold',
  },
  items: {
    marginTop: 30,
  },
  writeTaskWrapper: {
    position: 'absolute',
    bottom: 60,
    width: '100%',
    flexDirection: 'row',
    justifyContent: 'space-around',
    alignItems: 'center',
  },
  input: {
    paddingVertical: 15,
    paddingHorizontal: 15,
    backgroundColor: '#FFF',
    borderRadius: 60,
    borderColor: '#C0C0C0',
    borderWidth: 1,
    width: '80%',
  },
  addWrapper: {
    width: 60,
    height: 60,
    backgroundColor: '#FFF',
    borderRadius: 60,
    justifyContent: 'center',
    alignItems: 'center',
    borderColor: '#C0C0C0',
    borderWidth: 1,
  },
  addText: {},
});
