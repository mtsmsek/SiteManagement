/// <summary>Production environment settings (file replacement at build time).</summary>
export const environment = {
  production: true,
  // Overridden per-deployment; the Railway URL is wired in W6.
  apiBaseUrl: '/api-host-not-set',
};
