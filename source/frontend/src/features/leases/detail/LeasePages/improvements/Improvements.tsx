import { FieldArray, getIn, useFormikContext } from 'formik';
import { IFormLease } from 'interfaces';
import { ILeaseImprovement } from 'interfaces/ILeaseImprovement';

import Improvement from './components/Improvement/Improvement';
import * as Styled from './styles';

export interface IImprovementsProps {
  disabled?: boolean;
}

export const Improvements: React.FunctionComponent<
  React.PropsWithChildren<IImprovementsProps>
> = props => {
  const { values } = useFormikContext<IFormLease>();
  const improvements: ILeaseImprovement[] = getIn(values, 'improvements') ?? [];

  return (
    <Styled.ImprovementsContainer className="improvements">
      <FieldArray
        name="improvements"
        render={renderProps =>
          improvements.map((entry: ILeaseImprovement, index) => (
            <Improvement
              {...renderProps}
              nameSpace={`improvements.${index}`}
              disabled={props.disabled}
            />
          ))
        }
      ></FieldArray>
    </Styled.ImprovementsContainer>
  );
};

export default Improvements;
