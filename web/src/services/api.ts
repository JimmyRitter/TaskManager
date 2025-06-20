import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5161/api', // Adjust port if needed
  headers: {
    'Content-Type': 'application/json',
  },
})

export function setAuthToken(token: string | null) {
  if (token) {
    api.defaults.headers.common['Authorization'] = `Bearer ${token}`
  } else {
    delete api.defaults.headers.common['Authorization']
  }
}

export default api 