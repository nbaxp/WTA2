import Mock from 'better-mock';

function useToken() {
  Mock.mock('/api/token/login', 'POST', (request) => {
    const { userName, password } = JSON.parse(request.body ?? '{}');
    if (!userName) {
      return { code: 400, message: '用户名不能为空' };
    }
    if (!password) {
      return { code: 400, message: '密码不能为空' };
    }
    if (userName === 'admin' && password === '123456') {
      return new Promise((resolve) => {
        const token = 'token';
        const payload = 'payload';
        resolve({
          code: 200,
          data: { token, payload },
        });
      });
    }
  });
}

export default useToken;
