import Mock from 'better-mock';
import useToken from './token.js';

function useMock() {
  Mock.setup({
    timeout: '200-600',
  });
  useToken();
}

export default useMock;
