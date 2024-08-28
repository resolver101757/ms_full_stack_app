<template>
  <div>
    <h2>User List</h2>
    <div v-if="error">{{ error }}</div>
    <q-list v-else-if="users.length">
      <q-item v-for="user in users" :key="user.id">
        <q-item-section>
          <q-item-label>{{ user.name || user.id }}</q-item-label>
          <q-item-label caption>{{ user.email }}</q-item-label>
        </q-item-section>
      </q-item>
    </q-list>
    <div v-else>Loading users...</div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import axios from 'axios';

interface User {
  id: number;
  name: string;
  email: string;
}

const users = ref<User[]>([]);
const error = ref<string | null>(null);

const fetchUsers = async () => {
  try {
    console.log('Attempting to fetch users');
    const response = await axios.get<User[]>('http://localhost:5000/api/user');
    console.log('Successful response:', response.data);
    users.value = response.data;
  } catch (err: unknown) {
    console.error('Error fetching users:', err);
    if (axios.isAxiosError(err)) {
      console.error('Axios error details:', err.response?.data);
      error.value = `Error fetching users: ${err.message}. Status: ${err.response?.status}`;
    } else if (err instanceof Error) {
      error.value = `Error fetching users: ${err.message}`;
    } else {
      error.value = 'An unknown error occurred while fetching users.';
    }
  }
};

onMounted(() => {
  fetchUsers();
});
</script>