<template>
  <div class="bg-white">
    <!-- Hero Section -->
    <section class="relative bg-gradient-to-br from-orange-50 via-orange-100 to-orange-200 min-h-screen flex items-center">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-20">
        <div class="text-center">
          <!-- Main Headline -->
          <h1 class="text-4xl sm:text-5xl lg:text-6xl font-bold text-gray-900 mb-6 leading-tight">
            Your Productivity
            <span class="text-transparent bg-clip-text bg-gradient-to-r from-orange-500 to-orange-600">Journey</span>
            <br />Starts Here
          </h1>
          
          <!-- Subtitle -->
          <p class="text-xl sm:text-2xl text-gray-600 mb-12 max-w-3xl mx-auto leading-relaxed">
            Transform chaos into clarity. Master your tasks and unlock your full potential.
          </p>

          <!-- CTA Buttons -->
          <div class="flex flex-col sm:flex-row gap-4 justify-center items-center mb-20">
            <RouterLink 
              to="/register" 
              class="bg-gradient-to-r from-orange-500 to-orange-600 text-white px-8 py-4 rounded-full text-lg font-semibold hover:from-orange-600 hover:to-orange-700 transform hover:scale-105 transition-all duration-200 shadow-lg hover:shadow-xl"
            >
              Start Your Journey
            </RouterLink>
            <RouterLink 
              to="/login" 
              class="border-2 border-orange-500 text-orange-600 px-8 py-4 rounded-full text-lg font-semibold hover:bg-orange-500 hover:text-white transform hover:scale-105 transition-all duration-200"
            >
              Sign In
            </RouterLink>
          </div>

          <!-- Hero Stats -->
          <div class="grid grid-cols-1 sm:grid-cols-3 gap-8 max-w-2xl mx-auto">
            <div class="text-center">
              <div class="text-3xl font-bold text-orange-600 mb-2">10x</div>
              <div class="text-gray-600">More Productive</div>
            </div>
            <div class="text-center">
              <div class="text-3xl font-bold text-orange-600 mb-2">Zero</div>
              <div class="text-gray-600">Missed Deadlines</div>
            </div>
            <div class="text-center">
              <div class="text-3xl font-bold text-orange-600 mb-2">100%</div>
              <div class="text-gray-600">Goal Achievement</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Floating Elements -->
      <div class="absolute top-20 left-10 w-20 h-20 bg-orange-200 rounded-full opacity-60 animate-bounce"></div>
      <div class="absolute bottom-40 right-20 w-16 h-16 bg-orange-300 rounded-full opacity-40 animate-pulse"></div>
      <div class="absolute top-1/2 right-10 w-12 h-12 bg-orange-400 rounded-full opacity-50 animate-ping"></div>
    </section>

    <!-- Simple Features Section -->
    <section class="py-20 bg-white">
      <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-3xl sm:text-4xl font-bold text-gray-900 mb-4">
            Everything You Need
          </h2>
          <p class="text-xl text-gray-600">
            Simple tools. Powerful results.
          </p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-3 gap-12">
          <!-- Feature 1 -->
          <div class="text-center group">
            <div class="bg-orange-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-6 group-hover:bg-orange-200 transition-colors duration-200">
              <svg class="w-8 h-8 text-orange-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"></path>
              </svg>
            </div>
            <h3 class="text-xl font-semibold text-gray-900 mb-3">Lightning Fast</h3>
            <p class="text-gray-600">Capture thoughts instantly. Organize effortlessly.</p>
          </div>

          <!-- Feature 2 -->
          <div class="text-center group">
            <div class="bg-orange-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-6 group-hover:bg-orange-200 transition-colors duration-200">
              <svg class="w-8 h-8 text-orange-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"></path>
              </svg>
            </div>
            <h3 class="text-xl font-semibold text-gray-900 mb-3">Smart Insights</h3>
            <p class="text-gray-600">Track progress. See patterns. Celebrate wins.</p>
          </div>

          <!-- Feature 3 -->
          <div class="text-center group">
            <div class="bg-orange-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-6 group-hover:bg-orange-200 transition-colors duration-200">
              <svg class="w-8 h-8 text-orange-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6V4m0 2a2 2 0 100 4m0-4a2 2 0 110 4m-6 8a2 2 0 100-4m0 4a2 2 0 100 4m0-4v2m0-6V4m6 6v10m6-2a2 2 0 100-4m0 4a2 2 0 100 4m0-4v2m0-6V4"></path>
              </svg>
            </div>
            <h3 class="text-xl font-semibold text-gray-900 mb-3">Your Way</h3>
            <p class="text-gray-600">Fully customizable. Adapts to your workflow.</p>
          </div>
        </div>
      </div>
    </section>

    <!-- Social Proof Section -->
    <section class="py-20 bg-gray-50">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-12">
          <h2 class="text-3xl font-bold text-gray-900 mb-4">
            Join Thousands of Happy Users
          </h2>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
          <!-- Testimonial 1 -->
          <div class="bg-white p-8 rounded-2xl shadow-sm">
            <div class="flex items-center mb-4">
              <div class="w-12 h-12 bg-orange-500 rounded-full flex items-center justify-center text-white font-bold text-lg">
                S
              </div>
              <div class="ml-4">
                <div class="font-semibold text-gray-900">Sarah Chen</div>
                <div class="text-gray-600 text-sm">Entrepreneur</div>
              </div>
            </div>
            <p class="text-gray-700 italic">"TaskManager transformed my chaotic schedule into a well-oiled machine. I've never been more focused!"</p>
          </div>

          <!-- Testimonial 2 -->
          <div class="bg-white p-8 rounded-2xl shadow-sm">
            <div class="flex items-center mb-4">
              <div class="w-12 h-12 bg-orange-500 rounded-full flex items-center justify-center text-white font-bold text-lg">
                M
              </div>
              <div class="ml-4">
                <div class="font-semibold text-gray-900">Mike Rodriguez</div>
                <div class="text-gray-600 text-sm">Project Manager</div>
              </div>
            </div>
            <p class="text-gray-700 italic">"Finally, a tool that actually makes me more productive instead of just busier. Game changer!"</p>
          </div>
        </div>
      </div>
    </section>

    <!-- Final CTA Section -->
    <section class="py-20 bg-gradient-to-br from-gray-900 to-gray-800 text-white">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
        <h2 class="text-3xl sm:text-4xl font-bold mb-6">
          Ready to Get Started?
        </h2>
        <p class="text-xl mb-10 opacity-90">
          Your most productive self is just one click away.
        </p>
        
        <div class="flex flex-col sm:flex-row gap-4 justify-center items-center">
          <RouterLink 
            to="/register" 
            class="bg-gradient-to-r from-orange-500 to-orange-600 text-white px-10 py-4 rounded-full text-lg font-semibold hover:from-orange-600 hover:to-orange-700 transform hover:scale-105 transition-all duration-200 shadow-lg hover:shadow-xl"
          >
            Start Free Today
          </RouterLink>
          <div class="text-sm opacity-75">
            No credit card required
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { RouterLink } from 'vue-router'
</script>

<style scoped>
.landing-hero {
  width: 100%;
  max-width: 900px;
  margin: 0 auto;
  text-align: center;
  padding: 4rem 1rem 2rem 1rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}
.landing-hero h1 {
  font-size: 2.8rem;
  font-weight: bold;
  margin-bottom: 1rem;
  color: #42b983;
}
.subtitle {
  font-size: 1.3rem;
  color: var(--vt-c-text-light-2);
  margin-bottom: 2.5rem;
}
.info-blocks {
  display: flex;
  flex-wrap: wrap;
  gap: 2rem;
  justify-content: center;
  width: 100%;
}
.info {
  background: var(--color-background-soft);
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(60,60,60,0.06);
  padding: 2rem 1.5rem;
  flex: 1 1 300px;
  min-width: 260px;
  max-width: 400px;
  text-align: left;
}
.info h2 {
  color: #2c3e50;
  margin-bottom: 0.7rem;
  font-size: 1.3rem;
}
.info p {
  color: var(--vt-c-text-light-2);
  font-size: 1rem;
}
@media (max-width: 800px) {
  .info-blocks {
    flex-direction: column;
    gap: 1.5rem;
    align-items: center;
  }
  .landing-hero {
    padding: 2rem 0.5rem;
  }
}
</style> 