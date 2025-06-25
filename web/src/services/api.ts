import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5161/api', // Adjust port if needed
  headers: {
    'Content-Type': 'application/json',
  },
})

// Store for logout callback to avoid circular imports
let logoutCallback: (() => void) | null = null
let redirectCallback: (() => void) | null = null

export function setLogoutCallback(callback: () => void) {
  logoutCallback = callback
}

export function setRedirectCallback(callback: () => void) {
  redirectCallback = callback
}

export function setAuthToken(token: string | null) {
  if (token) {
    api.defaults.headers.common['Authorization'] = `Bearer ${token}`
  } else {
    delete api.defaults.headers.common['Authorization']
  }
}

// Response interceptor to handle token expiration
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // Check if error is due to expired/invalid token
    if (error.response?.status === 401 || error.response?.status === 403) {
      // Clear the authorization header
      delete api.defaults.headers.common['Authorization']
      
      // Call logout callback if available
      if (logoutCallback) {
        console.log('Token expired or invalid, logging out...')
        logoutCallback()
      }
      
      // Redirect to login page
      if (redirectCallback) {
        redirectCallback()
      }
    }
    return Promise.reject(error)
  }
)

export default api 