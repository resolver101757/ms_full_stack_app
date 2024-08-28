<template>
  <q-page padding>
    <h1>Customer Accounts</h1>
    
    <!-- Add User Form -->
    <q-form @submit="addUser" class="q-gutter-md">
      <q-input v-model="newUser.name" label="Name" required />
      <q-input v-model="newUser.email" label="Email" type="email" required />
      <q-btn label="Add User" type="submit" color="primary" />
    </q-form>

    <!-- User List -->
    <q-list separator>
      <q-item v-for="user in users" :key="user.id">
        <q-item-section>
          <q-item-label>{{ user.name }}</q-item-label>
          <q-item-label caption>{{ user.email }}</q-item-label>
        </q-item-section>
        <q-item-section side>
          <q-btn flat round color="primary" icon="edit" @click="editUser(user)" />
          <q-btn flat round color="negative" icon="delete" @click="removeUser(user.id)" />
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
          <q-form @submit="updateUser" class="q-gutter-md">
            <q-input v-model="editedUser.name" label="Name" required />
            <q-input v-model="editedUser.email" label="Email" type="email" required />
            <q-btn label="Update" type="submit" color="primary" />
          </q-form>
        </q-card-section>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';

interface User {
  id: number;
  name: string;
  email: string;
}

const users = ref<User[]>([]);
const newUser = reactive({ name: '', email: '' });
const editedUser = reactive({ id: 0, name: '', email: '' });
const editDialog = ref(false);

function addUser() {
  const id = users.value.length + 1;
  users.value.push({ id, ...newUser });
  newUser.name = '';
  newUser.email = '';
}

function editUser(user: User) {
  Object.assign(editedUser, user);
  editDialog.value = true;
}

function updateUser() {
  const index = users.value.findIndex(u => u.id === editedUser.id);
  if (index !== -1) {
    users.value[index] = { ...editedUser };
  }
  editDialog.value = false;
}

function removeUser(id: number) {
  users.value = users.value.filter(u => u.id !== id);
}
</script>