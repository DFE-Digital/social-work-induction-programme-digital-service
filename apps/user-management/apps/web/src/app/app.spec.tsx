import { expect, describe, test } from 'vitest';
import { render } from '@testing-library/preact';

import App from './app';

describe('AppElement', () => {
  test( 'should create successfully', () => {
    const app = render(<App />);
    expect(app).toBeTruthy();
  });
});
