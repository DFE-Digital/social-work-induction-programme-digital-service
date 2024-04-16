import { render } from 'preact'
import App from './app/app'
import './styles.scss'

// eslint-disable-next-line @typescript-eslint/no-non-null-assertion
render(<App />, document.getElementById('app')!)
