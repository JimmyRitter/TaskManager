import api from './api'

export async function createList(name: string, description = '') {
  const { data } = await api.post('/lists', { name, description })
  return data.data
}

export async function getUserLists() {
  const { data } = await api.get('/lists/get-all')
  return data.data
} 