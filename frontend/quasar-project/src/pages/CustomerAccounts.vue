<template>
  <q-page padding>
    <h1>Customer Accounts</h1>
    
    <!-- Add User Form -->
    <q-form @submit.prevent="addUser" class="q-gutter-md">
      <q-input v-model="newUser.Name" label="Username" required />
      <q-input v-model="newUser.email" label="Email" type="email" required />
      <q-btn label="Add User" type="submit" color="primary" />
    </q-form>

    <!-- User List -->
    <q-list separator>
      <q-item v-for="user in users" :key="user.id">
        <q-item-section>
          <q-item-label>{{ user.Name }}</q-item-label>
          <q-item-label caption>{{ user.email }}</q-item-label>
        </q-item-section>
        <q-item-section side>
          <q-btn flat round color="primary" icon="edit" @click="editUser(user)" />
          <q-btn flat round color="negative" icon="delete" @click="user.id !== undefined && removeUser(user.id)" />
        </q-item-section>
      </q-item>
    </q-list>

    <!-- Edit User Dialog -->
    <q-dialog v-model="editDialog">
      <q-card style="min-width: 350px">
        <q-card-section>
          <div class="text-h6">Edit User</div>
        </q-card-section>
        <q-card-section>
          <q-form @submit.prevent="updateUser" class="q-gutter-md">
            <q-input v-model="editedUser.Name" label="Username" required />
            <q-input v-model="editedUser.email" label="Email" type="email" required />
            <q-btn label="Update" type="submit" color="primary" />
          </q-form>
        </q-card-section>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import axios from 'axios';

interface User {
  id?: number;
  Name?: string;
  email?: string;
  Username?: string; // Add this line
}

const users = ref<User[]>([]);
const newUser = reactive({ Name: '', email: '' });
const editedUser = ref<User>({});
const editDialog = ref(false);

const apiUrl = 'http://localhost:5000/api/user'; 

async function fetchUsers() {
  const response = await axios.get<User[]>(apiUrl);
  users.value = response.data;
}

async function addUser() {
  const response = await axios.post<User>(apiUrl, newUser);
  users.value.push(response.data);
  newUser.Name = '';  
  newUser.email = '';
}

function editUser(user: User) {
  editedUser.value = { ...user, Username: user.Name };
  editDialog.value = true;
}

async function updateUser() {
  if (!editedUser.value.Name) {  
    console.error('Username is required');
    return;
  }
  if (editedUser.value.id === undefined) {
    console.error('User ID is missing');
    return;
  }
  try {
    const response = await axios.put(`${apiUrl}/${editedUser.value.id}`, {
      Username: editedUser.value.Name,
      Email: editedUser.value.email
    });
    const index = users.value.findIndex(u => u.id === editedUser.value.id);
    if (index !== -1) {
      users.value[index] = response.data;
    }
    editDialog.value = false;
  } catch (error) {
    console.error('Error updating user:', error);
    // Handle error (e.g., show error message to user)
  }
}

async function removeUser(id: number) {
  await axios.delete(`${apiUrl}/${id}`);
  users.value = users.value.filter(u => u.id !== id);
}

onMounted(fetchUsers);
</script>