export const config = process.env.REACT_APP_USE_MULTI_LOCAL_ENDPOINTS
  ? {
      endpoints: ["http://localhost:5000/game", "http://localhost:5001/game"],
    }
  : {
      endpoints: ["http://localhost:5000/game"],
    };
