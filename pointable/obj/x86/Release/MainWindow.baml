   M S B A M L   `   `   `  ����   	Pointable    PointableUI.MainWindow   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    8x,http://schemas.microsoft.com/winfx/2006/xaml    -     � Title$  	Pointable��$	��700��$
��1024��$
��1024��$	��700�� � WindowStartupLocation$ CenterScreen=� � 
ResizeMode$ CanMinimize=� � Icon$2 ,/Pointable;component/Resources/pointable.ico��.��� $	��603��$
��1057�� � ColumnDefinitions �� $
��550* ��� $
��507* �.+��� -   $��	imageBall��$	��670��$
��Left=�$��-58,-39,0,0q�$��Uniform=�$	��Top=�$	��670��$9��3/Pointable;component/Resources/Pointables_ball2.png��$��2�� � DataContext �� �� -   $��imageTop��$	��128��$
��Left=�$��
210,81,0,0q�$
��Fill=�$	��Top=�$	��128�� J� Opacity$ 1Q��� -   $��	borderTop��
������ $��1q�$	��128��$
��Left=�$��
209,80,0,0q�$	��Top=�$	��128��
������ �� -   $��
imageRight��$	��128��$
��Left=�$��363,230,0,0q�$
��Fill=�$	��Top=�$	��128���� -   $��borderRight��
������ $��1q�$	��128��$
��Left=�$��362,229,0,0q�$	��Top=�$	��128��
������ �� -   $��imageBottom��$	��128��$
��Left=�$��210,384,0,0q�$
��Fill=�$	��Top=�$	��128���� -   $��borderBottom��
������ $��1q�$	��128��$
��Left=�$��209,383,0,0q�$	��Top=�$	��128��
������ �� -	   $��	imageLeft��$	��128��$
��Left=�$��
61,230,0,0q�$
��Fill=�$	��Top=�$	��128���� -
   $��
borderLeft��
������ $��1q�$	��128��$
��Left=�$��
60,229,0,0q�$	��Top=�$	��128��
������ �� -   $��imageCenter��$	��128��$
��Left=�$��210,230,0,0q�$
��Fill=�$	��Top=�$	��128���� -   $��borderCenter��
������ $��1q�$	��128��$
��Left=�$��209,229,0,0q�$	��Top=�$	��128��
������ � $��17,341,67,8q�$	��423��$��1�� �� $
��188* ��� $
��125* ��� $
��Auto �.+��� -   $��label3��$��Action��$��Segoe)�$��32(�$��50��$
��Left=�$��11,0,0,0q�$	��Top=�$	��243��$��2���� -   $��tabControlAction��$	��197��$
��Left=�$��	12,52,0,0q�$	��Top=�$	��411��$��3�����	#00000000.@��� -   $��tabItem1��$��Launch Application��.��� .+��� -   $��comboBoxApplication��$��12(�$��28��$
��Left=�$��0,14,0,0q�$	��Top=�$	��298���� -   $��buttonApplicationAdd��$	��Add��$��25��$
��Left=�$��
305,14,0,0q�$	��Top=�$��90���� -   $��buttonApplicationEdit��$
��Edit��$��25��$
��Left=�$��
305,45,0,0q�$	��Top=�$��90���� -   $��buttonApplicationDelete��$��Delete��$��25��$
��Left=�$��
305,76,0,0q�$	��Top=�$��90��$y�Hidden=��� -   $��labelActionDescription��$��28��$��12(�$��0,13.52,124,0q�$	��Top=�
������ �� -   $��tabItem2��$��
Keystrokes�� �� 
IsSelected . .���  �� $	��74* ��� $	��17* ��� $
��310* �.+��� -   $��comboBoxKeystrokes��$��12(�$��28��$
��Left=�$��0,14,0,0q�$	��Top=�$	��298��$��3���� -   $��buttonKeystrokesAdd��$	��Add��$��25��$
��Left=�$��
214,14,0,0q�$	��Top=�$��90��$��2���� -   $��buttonKeystrokesEdit��$
��Edit��$��25��$
��Left=�$��
214,45,0,0q�$	��Top=�$��90��$��2���� -   $��buttonKeystrokesDelete��$��Delete��$y�Hidden=�$��25��$
��Left=�$��
214,76,0,0q�$	��Top=�$��90��$��2���� -   $%��labelActionKeystrokeDescription��$��28��$��12(�$��0,13.52,124,0q�$	��Top=�
������ $��3��� $��17,12,67,281q�$��1�� �� $
��316* ��� $
��111* �.+��� -   $��label4��$��
Pointables��$��Segoe)�$��32(�$��50��$��12,0,3.04,0q�$	��Top=��� -   $��buttonAddPointable��$	��Add��$��1��$��25��$
��Left=�$��9,57,0,0q�$	��Top=�$��90���� -   $��listBoxPointables��$��12,57,3.04,0q�$��16(�$	��241��$	��Top=����� �� -   $��buttonEditPointable��$
��Edit��$��1��$��25��$
��Left=�$��9,88,0,0q�$	��Top=�$��90���� -   $��buttonDeletePointable��$��Delete��$��1��$��8.96,151,11,134q��� -    $��buttonCalibrate��$��8.96,119,11,0q�$��1��$��25��$	��Top=�.��	Calibrate�� -!   $��buttonDisable��$��25��$��8.96,0,11,102q�$��Bottom=�$��1��.��	Disable�� -"   $��labelPointableCount��$��Segoe)�$��20(�$��0,278,77.96,293q�$��1��$��Right=�$��90��$��Center=�.��21/24