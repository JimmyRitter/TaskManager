<template>
  <section class="landing-hero">
    <h1>Welcome to TaskManager</h1>
    <p class="subtitle">Your all-in-one solution to organize tasks, lists, and boost your productivity.</p>
    <div class="info-blocks">
      <div class="info">
        <h2>Why TaskManager?</h2>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque euismod, nisi eu consectetur consectetur, nisl nisi consectetur nisi, euismod euismod nisi nisi euismod.</p>
      </div>
      <div class="info">
        <h2>Keep Your Productivity High</h2>
        <p>Stay on top of your goals, track your progress, and collaborate with others. Productivity is the key to success, and TaskManager helps you achieve it effortlessly.</p>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
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